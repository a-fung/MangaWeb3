// MangasModule.cs
//

using System;
using System.Collections.Generic;
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

        private static MangasModule _instance = null;
        public static MangasModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MangasModule();
                }

                return _instance;
            }
        }

        private MangasModule()
            : base("mangas-module")
        {
            pagination = new Pagination(jQuery.Select(".mangas-pagination", attachedObject), ChangePage, GetTotalPage, "centered");
            Refresh();
        }

        protected override void OnShow()
        {
        }

        [AlternateSignature]
        public extern void Refresh();
        public void Refresh(MangaFilter filter)
        {
            Show();

            if (Script.IsNullOrUndefined(filter))
            {
                filter = new MangaFilter();
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
            }

            MangaListRequest request = new MangaListRequest();
            request.filter = filter;
            Request.Send(request, MangaListRequestSuccess);
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
            items = response.items;
            ChangePage(1);
            pagination.Refresh(true);
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
            int j = 0;
            jQueryObject row = null;
            jQuery.Select("#mangas-list").Children().Remove();
            for (int i = (page - 1) * Environment.ElementsPerPage; i < items.Length && i < page * Environment.ElementsPerPage; i++)
            {
                if (j % Environment.MangaListItemPerRow == 0)
                {
                    row = Template.Get("client", "mangas-list-row", true).AppendTo(jQuery.Select("#mangas-list"));
                }

                new MangaListItem(row, items[i]);
                j++;
            }
        }
    }
}