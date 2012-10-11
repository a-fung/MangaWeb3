// ReadModule.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Client.Widget;
using afung.MangaWeb3.Common;
using jQueryApi;
using System.Runtime.CompilerServices;

namespace afung.MangaWeb3.Client.Module
{
    public class ReadModule : ModuleBase
    {
        private const int RefreshMangaAreaInterval = 150;

        private static ReadModule _instance = null;
        private static ReadModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ReadModule();
                }

                return _instance;
            }
        }

        private MangaReadResponse manga;
        private jQueryObject mangaArea;
        private jQueryObject infoArea;
        private jQueryObject buttonArea;

        private int _currentPage;
        private int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                jQueryBootstrap infoPage = (jQueryBootstrap)jQuery.Select("#read-info-page").Attribute("data-original-title", (value + 1).ToString());
                infoPage.Tooltip("fixTitle");

                if (infoArea.Is(":visible"))
                {
                    if (_currentPage != value)
                    {
                        infoPage.Tooltip("show");
                    }
                }
                else
                {
                    infoPage.Tooltip("hide");
                }

                _currentPage = value;
            }
        }

        private int oldWidth;
        private bool readNext;
        private Dictionary<int, MangaPage> loadedPages = new Dictionary<int, MangaPage>();
        private Dictionary<int, MangaPage> insertedPages = new Dictionary<int, MangaPage>();
        private Queue<MangaPage> loadQueue = new Queue<MangaPage>();
        private bool loadingPage;
        private Date lastRefreshMangaArea;
        private int pagesHead = 0;
        private int pagesTail = 0;
        private int _offset;

        private int Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = Math.Round(value);
                if (jQuery.Browser.MSIE && float.Parse(jQuery.Browser.Version) < 10)
                {
                    string cssValue = "translate(" + _offset + "px,0)";
                    mangaArea.CSS("transform", cssValue).CSS("-ms-transform", cssValue);
                }
                else
                {
                    string cssValue = "translate3d(" + _offset + "px,0,0)";
                    mangaArea.CSS("transform", cssValue).CSS("-ms-transform", cssValue).CSS("-moz-transform", cssValue).CSS("-webkit-transform", cssValue).CSS("-o-transform", cssValue);
                }
            }
        }

        private int touchInitialOffset;
        private int touchInitialXPosition;

        private ReadModule()
            : base("client", "read-module")
        {
            readNext = false;
            mangaArea = jQuery.Select("#read-manga-area-inner");
            infoArea = jQuery.Select("#read-info-area");
            buttonArea = jQuery.Select("#read-button-area");

            ((jQueryObjectTouch)jQuery.Select("#read-manga-area"))
                .TouchInitialize(new Dictionary<string, object>("maxtouch", 1))
                .Bind("touch_start", TouchHandler)
                .Bind("touch_move", TouchHandler)
                .Bind("touch_end", TouchHandler);

            // prevent default scrolling in iOS
            attachedObject.Bind(
                "touchmove",
                delegate(jQueryEvent e)
                {
                    e.PreventDefault();
                });

            jQuery.Select("#read-info-btn").Click(ToggleInfoButtonClicked);
        }

        protected override void OnShow()
        {
            HideInfo();
        }

        private void HideInfo()
        {
            infoArea.Hide();
            ((jQueryBootstrap)jQuery.Select("#read-info-page")).Tooltip("hide");
            buttonArea.RemoveClass("show");
        }

        private void ShowInfo()
        {
            infoArea.Show();
            ((jQueryBootstrap)jQuery.Select("#read-info-page")).Tooltip("show");
            buttonArea.AddClass("show");
        }

        private void ToggleInfo()
        {
            if (infoArea.Is(":visible"))
            {
                HideInfo();
            }
            else
            {
                ShowInfo();
            }
        }

        private void ToggleInfoButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            ToggleInfo();
        }

        public static void ReadManga(MangaReadResponse manga)
        {
            if (manga.id == -1)
            {
                ErrorModal.ShowError(Strings.Get("MangaNotAvailable"));
            }
            else
            {
                Instance.Read(manga);
            }
        }

        public void Read(MangaReadResponse manga)
        {
            this.manga = manga;
            Show();

            CurrentPage = -1;
            InitializeRead();
        }

        private void InitializeRead()
        {
            RemoveAllPages();
            UnloadAllPages();
            attachedObject.Height(jQuery.Window.GetHeight());
            oldWidth = attachedObject.GetWidth();

            if (CurrentPage == -1)
            {
                if (readNext)
                {
                    CurrentPage = 0;
                }
                else
                {
                    CurrentPage = Settings.GetCurrentPage(manga.id);
                    if (!Number.IsFinite(CurrentPage))
                    {
                        CurrentPage = 0;
                    }
                }
            }

            readNext = true;

            NavigateTo(CurrentPage);
        }

        private void NavigateTo(int page)
        {
            if (page < 0 || page >= manga.pages)
            {
                return;
            }

            RemoveAllPages();

            CurrentPage = page;

            LoadPages(page);
            RefreshMangaArea();
        }

        private void RemoveAllPages()
        {
            List<int> keys = new List<int>();
            foreach (int key in insertedPages.Keys)
            {
                keys.Add(key);
            }

            foreach (int key in keys)
            {
                Remove(insertedPages[key]);
            }
        }

        [AlternateSignature]
        private extern void RefreshMangaArea();
        private void RefreshMangaArea(bool force)
        {
            if (Script.IsNullOrUndefined(force)) force = true;
            if (!force && !Script.IsNullOrUndefined(lastRefreshMangaArea) && Date.Now - lastRefreshMangaArea <= RefreshMangaAreaInterval)
            {
                return;
            }

            lastRefreshMangaArea = Date.Now;

            if (insertedPages.Count == 0)
            {
                MangaPage page = GetMangaPage(CurrentPage);
                if (!page.Loaded) return;
                Insert(page);
                RefreshMangaArea();
                return;
            }

            MangaPage lastPage = GetMangaPage(pagesTail);
            MangaPage lastPagePlusOne;
            if (pagesTail != manga.pages - 1 && (lastPagePlusOne = GetMangaPage(pagesTail + 1)).Loaded && Offset + lastPage.Offset + attachedObject.GetWidth() / 2 > 0)
            {
                Insert(lastPagePlusOne);
                RefreshMangaArea();
                LoadPages(pagesTail);
                return;
            }

            MangaPage firstPage = GetMangaPage(pagesHead);
            MangaPage firstPageMinusOne;
            if (pagesHead != 0 && (firstPageMinusOne = GetMangaPage(pagesHead - 1)).Loaded && Offset + firstPage.Offset + firstPage.Width < attachedObject.GetWidth() * 1.5)
            {
                Insert(firstPageMinusOne);
                RefreshMangaArea();
                LoadPages(pagesHead);
                return;
            }

            if (insertedPages.Count > 1)
            {
                if (Offset + lastPage.Offset + lastPage.Width + attachedObject.GetWidth() < 0)
                {
                    Remove(lastPage);
                    UnloadPage(lastPage.Page + Environment.MangaPagesUnloadDistance);
                    RefreshMangaArea();
                    return;
                }

                if (Offset + firstPage.Offset > attachedObject.GetWidth() * 2)
                {
                    Remove(firstPage);
                    UnloadPage(firstPage.Page - Environment.MangaPagesUnloadDistance);
                    RefreshMangaArea();
                    return;
                }
            }

            int newCurrentPage = -1;
            int viewCenter = attachedObject.GetWidth() / 2;
            foreach (int page in insertedPages.Keys)
            {
                int left = Offset + insertedPages[page].Offset;
                int right = left + insertedPages[page].Width;
                if (left < viewCenter && right > viewCenter)
                {
                    newCurrentPage = int.Parse(page.ToString(), 10);
                    break;
                }
            }

            if (newCurrentPage >= 0 && newCurrentPage < manga.pages && CurrentPage != newCurrentPage)
            {
                CurrentPage = newCurrentPage;
            }
        }

        private void LoadPages(int page)
        {
            for (int i = 0; i <= Environment.MangaPagesToPreload; i++)
            {
                LoadPage(page + i);
                LoadPage(page - i);
            }
        }

        private void LoadPage(int page)
        {
            if (page < 0 || page >= manga.pages)
            {
                return;
            }

            MangaPage mangaPage;
            if (!loadQueue.Contains(mangaPage = GetMangaPage(page)))
            {
                loadQueue.Enqueue(mangaPage);
            }

            LoadNextPage();
        }

        private void LoadNextPage()
        {
            if (loadingPage)
            {
                return;
            }

            if (loadQueue.Count < 1)
            {
                return;
            }

            MangaPage page = loadQueue.Dequeue();
            loadingPage = true;
            page.Load(delegate
            {
                loadingPage = false;
                LoadNextPage();
                RefreshMangaArea();
            });
        }

        private void UnloadPage(int page)
        {
            if (loadedPages.ContainsKey(page))
            {
                loadedPages[page].Unload();
                ((List<MangaPage>)(object)loadQueue).Remove(loadedPages[page]);
                loadedPages.Remove(page);
            }
        }

        private void UnloadAllPages()
        {
            loadQueue.Clear();

            List<int> keys = new List<int>();
            foreach (int key in loadedPages.Keys)
            {
                keys.Add(key);
            }

            foreach (int key in keys)
            {
                UnloadPage(key);
            }

            loadingPage = false;
        }

        private MangaPage GetMangaPage(int page)
        {
            if (loadedPages.ContainsKey(page))
            {
                return loadedPages[page];
            }
            else
            {
                return loadedPages[page] = new MangaPage(manga.id, page, attachedObject.GetWidth(), attachedObject.GetHeight());
            }
        }

        private void Insert(MangaPage page)
        {
            if (insertedPages.Count == 0)
            {
                pagesTail = pagesHead = page.Page;
                page.AppendTo(mangaArea, 0, 0, page);

                if (attachedObject.GetWidth() > page.Width)
                {
                    Offset = (jQuery.Window.GetWidth() - page.Width) / 2;
                }
                else
                {
                    Offset = 0;
                }
            }
            else if (page.Page == pagesTail + 1)
            {
                MangaPage lastPage = GetMangaPage(pagesTail);
                page.AppendTo(mangaArea, lastPage.Offset, -1, page);
                pagesTail = page.Page;
            }
            else if (page.Page == pagesHead - 1)
            {
                MangaPage firstPage = GetMangaPage(pagesHead);
                page.AppendTo(mangaArea, firstPage.Offset, 1, firstPage);
                pagesHead = page.Page;
            }
            else
            {
                return;
            }

            insertedPages[page.Page] = page;
        }

        private void Remove(MangaPage page)
        {
            insertedPages.Remove(page.Page);
            page.Remove();
            if (page.Page == pagesTail) pagesTail--;
            if (page.Page == pagesHead) pagesHead++;
        }

        [AlternateSignature]
        private extern void TouchHandler(jQueryEvent e);
        private void TouchHandler(jQueryTouchEvent e)
        {
            if (e.Type == "touch_start")
            {
                touchInitialOffset = Offset;
                touchInitialXPosition = e.ClientX;
            }
            else
            {
                MangaPage firstPage;
                int targetOffset = e.ClientX - touchInitialXPosition + touchInitialOffset;
                int minOffset = attachedObject.GetWidth() / 2 - (firstPage = GetMangaPage(pagesHead)).Offset - firstPage.Width;
                int maxOffset = attachedObject.GetWidth() / 2 - GetMangaPage(pagesTail).Offset;
                Offset = targetOffset < minOffset ? minOffset : targetOffset > maxOffset ? maxOffset : targetOffset;
                RefreshMangaArea(false);
            }
        }
    }
}
