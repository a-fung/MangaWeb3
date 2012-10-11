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

        private int currentPage;
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

        private ReadModule()
            : base("client", "read-module")
        {
            readNext = false;
            mangaArea = jQuery.Select("#read-manga-area-inner");
        }

        protected override void OnShow()
        {
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

            currentPage = -1;
            InitializeRead();
        }

        private void InitializeRead()
        {
            RemoveAllPages();
            UnloadAllPages();
            attachedObject.Height(jQuery.Window.GetHeight());
            oldWidth = attachedObject.GetWidth();

            if (currentPage == -1)
            {
                if (readNext)
                {
                    currentPage = 0;
                }
                else
                {
                    currentPage = Settings.GetCurrentPage(manga.id);
                    if (!Number.IsFinite(currentPage))
                    {
                        currentPage = 0;
                    }
                }
            }

            readNext = true;

            NavigateTo(currentPage);
        }

        private void NavigateTo(int page)
        {
            if (page < 0 || page >= manga.pages)
            {
                return;
            }

            RemoveAllPages();

            currentPage = page;

            LoadPages(page);
            RefreshMangaArea();
        }

        private void RemoveAllPages()
        {
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
                MangaPage page = GetMangaPage(currentPage);
                if (!page.Loaded) return;
                Insert(page);
                RefreshMangaArea();
                return;
            }

            MangaPage lastPage = GetMangaPage(pagesTail);
            MangaPage lastPagePlusOne = GetMangaPage(pagesTail + 1);
            if (pagesTail != manga.pages - 1 && lastPagePlusOne.Loaded && Offset + lastPage.Offset + attachedObject.GetWidth() / 2 > 0)
            {
                Insert(lastPagePlusOne);
                RefreshMangaArea();
                LoadPages(pagesTail);
                return;
            }

            MangaPage firstPage = GetMangaPage(pagesHead);
            MangaPage firstPageMinusOne = GetMangaPage(pagesHead - 1);
            if (pagesHead != 0 && firstPageMinusOne.Loaded && Offset + firstPage.Offset + firstPage.Width < attachedObject.GetWidth() * 1.5)
            {
                Insert(firstPageMinusOne);
                RefreshMangaArea();
                LoadPages(pagesHead);
                return;
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

            loadQueue.Enqueue(GetMangaPage(page));
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

        private void UnloadAllPages()
        {
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
    }
}
