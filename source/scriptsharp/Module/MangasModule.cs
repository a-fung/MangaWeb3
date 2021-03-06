// MangasModule.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Widget;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Module
{
    public class MangasModule : ClientModuleBase
    {
        private MangaListItemJson[] items;
        private Pagination pagination;

        private string currentFolder = "";
        public string CurrentFolder
        {
            get
            {
                return currentFolder;
            }
        }

        private static MangasModule _instance = null;
        public static MangasModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    Action loginRefreshCallback = delegate
                    {
                        if (_instance != null)
                        {
                            _instance.Refresh(null, true);
                        }
                    };

                    _instance = new MangasModule(loginRefreshCallback);
                }

                return _instance;
            }
        }

        private int currentPage;

        private bool autoChangePage = false;
        private MangaFilter lastFilter = null;

        private MangasModule(Action loginRefreshCallback)
            : base("mangas-module", loginRefreshCallback)
        {
            items = new MangaListItemJson[] { };
            currentPage = 1;
            pagination = new Pagination(jQuery.Select(".mangas-pagination", attachedObject), ChangePage, GetTotalPage, "centered");
            jQuery.Document.Keyup(OnKeyUp);
            jQuery.Document.Click(DocumentClick);
            Refresh();
        }

        protected override void OnBeforeShow()
        {
            if (items.Length > 0)
            {
                jQuery.Select("#mangas-list").Children().Remove();
            }
        }

        protected override void OnShow()
        {
            if (items.Length > 0)
            {
                ChangePage(currentPage);
            }
        }

        [AlternateSignature]
        public extern void Refresh();
        [AlternateSignature]
        public extern void Refresh(MangaFilter filter);
        public void Refresh(MangaFilter filter, bool suppressShowModule)
        {
            if (Script.IsNullOrUndefined(filter))
            {
                filter = new MangaFilter();

                if (!String.IsNullOrEmpty(Settings.CurrentFolder))
                {
                    filter.folder = Settings.CurrentFolder;
                }
            }

            jQueryObject breadcrumb = jQuery.Select("#mangas-breadcrumb");
            breadcrumb.Children().Remove();

            if (!String.IsNullOrEmpty(filter.folder))
            {
                int i = 0, j = 0;
                string separator = Environment.ServerType == ServerType.AspNet ? "\\" : "/";

                while ((i = filter.folder.IndexOf(separator, j)) != -1)
                {
                    jQuery.Select(
                        ".mangas-breadcrumb-btn",
                        Template.Get("client", "mangas-breadcrumb-link", true).AppendTo(breadcrumb))
                        .Text(filter.folder.Substring(j, i))
                        .Attribute("data-folder", filter.folder.Substr(0, i))
                        .Click(BreadcrumbFolderClicked);

                    j = i + 1;

                    breadcrumb.Append(" ");
                }

                Template.Get("client", "mangas-breadcrumb-active-folder", true).AppendTo(breadcrumb).Text(filter.folder.Substr(j));
                Settings.CurrentFolder = currentFolder = filter.folder;
            }
            else if (!String.IsNullOrEmpty(filter.tag))
            {
                jQuery.Select(
                    ".mangas-breadcrumb-tag-name",
                    Template.Get("client", "mangas-breadcrumb-tag", true).AppendTo(breadcrumb))
                    .Text(filter.tag);
            }
            else if (filter.search != null)
            {
                Template.Get("client", "mangas-breadcrumb-active-folder", true).AppendTo(breadcrumb).Text(Strings.Get("SearchResult"));
            }
            else
            {
                Template.Get("client", "mangas-breadcrumb-active-folder", true).AppendTo(breadcrumb).Text(Strings.Get("AllMangas"));
                Settings.CurrentFolder = currentFolder = "";
            }

            Action onReady = delegate
            {
                MangaListRequest request = new MangaListRequest();
                lastFilter = request.filter = filter;
                Request.Send(request, MangaListRequestSuccess);
                Template.Get("client", "loading-well", true).AppendTo(jQuery.Select("#mangas-loading"));
                FoldersWidget.ExpandToFolder(currentFolder);
            };

            if (!attachedObject.Is(":visible"))
            {
                items = new MangaListItemJson[] { };
                jQuery.Select(".mangas-pagination").Children().Remove();
                jQuery.Select("#mangas-list").Children().Remove();
                if (suppressShowModule)
                {
                    onReady();
                }
                else
                {
                    Show(onReady);
                }
            }
            else
            {
                onReady();
            }
        }

        private void BreadcrumbFolderClicked(jQueryEvent e)
        {
            e.PreventDefault();
            string folder = jQuery.FromElement(e.Target).GetAttribute("data-folder");

            if (!String.IsNullOrEmpty(folder))
            {
                MangaFilter filter = new MangaFilter();
                filter.folder = folder;
                Refresh(filter);
            }
        }

        [AlternateSignature]
        private extern void MangaListRequestSuccess(JsonResponse response);
        private void MangaListRequestSuccess(MangaListResponse response)
        {
            jQuery.Select("#mangas-loading").Children().Remove();

            items = response.items;
            SortItems();

            ChangePage(1);
            pagination.Refresh(true);
        }

        public void SortItems()
        {
            if (items.Length == 0)
            {
                return;
            }

            CompareCallback<MangaListItemJson> compare = null;

            switch (Settings.Sort)
            {
                case 1:
                    compare = delegate(MangaListItemJson a, MangaListItemJson b)
                    {
                        return a.pages - b.pages;
                    };
                    break;
                case 2:
                    compare = delegate(MangaListItemJson a, MangaListItemJson b)
                    {
                        return (int)a.size - (int)b.size;
                    };
                    break;
                case 3:
                    compare = delegate(MangaListItemJson a, MangaListItemJson b)
                    {
                        return b.date - a.date;
                    };
                    break;
                case -1:
                    List<MangaListItemJson> list1 = (List<MangaListItemJson>)(object)items;
                    List<MangaListItemJson> list2 = new List<MangaListItemJson>();
                    int index;
                    while (list1.Count > 0)
                    {
                        index = Math.Floor(Math.Random() * list1.Count);
                        list2.Add(list1[index]);
                        list1.RemoveAt(index);
                    }

                    items = (MangaListItemJson[])(object)list2;
                    break;
                case 0:
                default:
                    compare = delegate(MangaListItemJson a, MangaListItemJson b)
                    {
                        return Utility.NaturalCompare(a.title, b.title);
                    };
                    break;
            }

            if (compare != null)
            {
                ((List<MangaListItemJson>)(object)items).Sort(compare);
            }
        }

        private int GetTotalPage()
        {
            if (items == null || items.Length == 0)
            {
                return 1;
            }

            return Math.Ceil(items.Length / Environment.ElementsPerPage);
        }

        private void ChangePage(int page)
        {
            currentPage = page;
            int j = 0;
            jQueryObject row = null;
            jQueryObject mangaList = jQuery.Select("#mangas-list");
            List<MangaListItem> listItems = new List<MangaListItem>();

            int loadedItems = 0;
            Action itemLoadFinishCallback = delegate
            {
                if (autoChangePage && page * Environment.ElementsPerPage < items.Length)
                {
                    loadedItems++;
                    if (loadedItems >= Environment.ElementsPerPage)
                    {
                        pagination.Refresh(false, currentPage + 1);
                        ChangePage(currentPage + 1);
                    }
                }
            };

            if (autoChangePage && page * Environment.ElementsPerPage >= items.Length)
            {
                DisableAutoChangePage();
            }

            Action onReady = delegate
            {
                mangaList.Children().Remove();
                if (items.Length == 0)
                {
                    jQueryObject noItem = Template.Get("client", "noitem-well", true).AppendTo(mangaList);
                    if (currentFolder == "")
                    {
                        Template.Get("client", "noguest-warning", true).AppendTo(noItem);
                    }
                }

                for (int i = (page - 1) * Environment.ElementsPerPage; i < items.Length && i < page * Environment.ElementsPerPage; i++)
                {
                    if (j % Environment.MangaListItemPerRow == 0)
                    {
                        row = Template.Get("client", "mangas-list-row", true).AppendTo(jQuery.Select("#mangas-list"));
                    }

                    listItems.Add(new MangaListItem(row, items[i], i + 1 < items.Length ? items[i + 1].id : -1, itemLoadFinishCallback));
                    j++;
                }
            };

            if (mangaList.Is(":visible") && Settings.UseAnimation)
            {
                Utility.OnTransitionEnd(
                    mangaList.AddClass("fade"),
                    delegate
                    {
                        mangaList.RemoveClass("fade");
                        onReady();
                    });
            }
            else
            {
                onReady();
            }
        }

        public int GetNextMangaId(int currentId)
        {
            for (int i = 0; i < items.Length - 1; i++)
            {
                if (currentId == items[i].id)
                {
                    return items[i + 1].id;
                }
            }

            return -1;
        }

        private void OnKeyUp(jQueryEvent keyEvent)
        {
            if (attachedObject.Is(":visible"))
            {
                if (Script.IsNullOrUndefined(keyEvent))
                {
                    keyEvent = (jQueryEvent)(object)Window.Event;
                }

                ElementEvent evt = (ElementEvent)(object)keyEvent;

                if (evt.CtrlKey && evt.AltKey && evt.KeyCode == 39)
                {
                    if (autoChangePage)
                    {
                        DisableAutoChangePage();
                    }
                    else
                    {
                        EnableAutoChangePage();
                    }
                }
            }
        }

        private void DocumentClick(jQueryEvent e)
        {
            if (attachedObject.Is(":visible") && autoChangePage)
            {
                DisableAutoChangePage();
            }
        }

        private void EnableAutoChangePage()
        {
            if (lastFilter != null)
            {
                autoChangePage = true;
                Document.Title = Strings.Get("AutoChangePage") + Document.Title;
                Refresh(lastFilter);
            }
        }

        private void DisableAutoChangePage()
        {
            autoChangePage = false;
            Document.Title = Document.Title.Replace(Strings.Get("AutoChangePage"), "");
        }
    }
}
