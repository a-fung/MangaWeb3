// ReadModule.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Client.Widget;
using afung.MangaWeb3.Common;
using jQueryApi;

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
        private jQueryObject sliderHandle;

        private bool sendingReadReqeust;
        private bool sendingDirectionReqeust;
        private bool hasHorizonalMouseWheelScroll;
        private bool inTransition;

        private int _currentPage;
        private int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                jQueryBootstrap infoPage = (jQueryBootstrap)jQuery.Select("#read-info-page").Attribute("data-original-title", (manga.ltr ? manga.pages - value : value + 1).ToString());
                infoPage.Tooltip("fixTitle");

                if (infoArea.Is(":visible"))
                {
                    if (_currentPage != value)
                    {
                        infoPage.Tooltip("show");
                    }

                    int maxOffset = sliderHandle.Parent().GetInnerWidth() - sliderHandle.GetOuterWidth();
                    SliderOffset = (int)(maxOffset - (maxOffset + 1) / manga.pages * (value + 0.5));
                }
                else
                {
                    infoPage.Tooltip("hide");
                }

                if (value >= 0 && value < manga.pages)
                {
                    Settings.SetCurrentPage(manga.id, manga.ltr ? manga.pages - value - 1 : value);
                }

                _currentPage = value;
            }
        }

        private int oldWidth;
        private bool readNext;
        private Dictionary<int, MangaPage> loadedPages = new Dictionary<int, MangaPage>();
        private Dictionary<int, MangaPage> insertedPages = new Dictionary<int, MangaPage>();
        private Queue<MangaPage> loadQueue = new Queue<MangaPage>();
        private Date lastRefreshMangaArea;
        private int pagesHead = 0;
        private int pagesTail = 0;

        private bool _loadingPage;
        private bool LoadingPage
        {
            get
            {
                return _loadingPage;
            }
            set
            {
                _loadingPage = value;
                if (Settings.UseAnimation)
                {
                    if (value)
                    {
                        jQuery.Select("#read-loading").AddClass("fade in");
                    }
                    else
                    {
                        jQuery.Select("#read-loading").RemoveClass("in");
                    }
                }
                else
                {
                    if (value)
                    {
                        jQuery.Select("#read-loading").Show();
                    }
                    else
                    {
                        jQuery.Select("#read-loading").Hide();
                    }
                }
            }
        }

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

        private int _sliderOffset;
        private int SliderOffset
        {
            get
            {
                return _sliderOffset;
            }
            set
            {
                _sliderOffset = Math.Round(value);
                if (jQuery.Browser.MSIE && float.Parse(jQuery.Browser.Version) < 10)
                {
                    string cssValue = "translate(" + _sliderOffset + "px,0)";
                    sliderHandle.CSS("transform", cssValue).CSS("-ms-transform", cssValue);
                }
                else
                {
                    string cssValue = "translate3d(" + _sliderOffset + "px,0,0)";
                    sliderHandle.CSS("transform", cssValue).CSS("-ms-transform", cssValue).CSS("-moz-transform", cssValue).CSS("-webkit-transform", cssValue).CSS("-o-transform", cssValue);
                }
            }
        }

        private bool inTouchHandler;
        private int touchScrollDirection;
        private int touchInitialOffset;
        private int touchInitialXPosition;
        private int touchInitialOffsetY;
        private int touchInitialYPosition;
        private List<int> lastTouchOffset;
        private List<Date> lastTouchTime;
        private List<int> lastTouchOffsetY;
        private List<Date> lastTouchTimeY;
        private MangaPage currentVerticalScrollPage;

        private bool inSliderTouchHandler;
        private int sliderTouchInitialOffset;
        private int sliderTouchInitialXPosition;

        private SelfClearingTimeout resizeTimeout = new SelfClearingTimeout();

        private ReadModule()
            : base("client", "read-module", false)
        {
            readNext = false;
            mangaArea = jQuery.Select("#read-manga-area-inner");
            infoArea = jQuery.Select("#read-info-area");
            buttonArea = jQuery.Select("#read-button-area");

            ((jQueryObjectTouch)jQuery.Select("#read-manga-area"))
                .TouchInitialize(new Dictionary<string, object>("maxtouch", 1))
                .Bind("touch_start touch_move touch_end", TouchHandler);

            sliderHandle = ((jQueryObjectTouch)jQuery.Select("#read-slider-handle"))
                .TouchInitialize(new Dictionary<string, object>("maxtouch", 1))
                .Bind("touch_start touch_move touch_end", SliderTouchHandler);

            // prevent default scrolling in iOS
            attachedObject.Bind(
                "touchmove",
                delegate(jQueryEvent e)
                {
                    e.PreventDefault();
                });

            jQuery.Select("#read-info-btn").Click(ToggleInfoButtonClicked);
            jQuery.Select("#read-exit-btn").Click(ExitButtonClicked);
            jQuery.Select("#read-next-btn").Click(NextButtonClicked);
            jQuery.Select(".arrow-btn").Click(ArrowButtonClicked);
            ((jQueryBootstrap)jQuery.Select("#read-direction-btn").Click(DirectionButtonClicked)).Tooltip(new Dictionary<string, object>("placement", "right", "title", Strings.Get("ChangeDirection")));
            jQuery.Document.Keyup(OnKeyUp);
            jQuery.Document.Bind("mousewheel DOMMouseScroll", MouseWheelHandler);
            jQuery.Window.Resize(OnResize);

            if (Settings.UseAnimation)
            {
                jQuery.Select("#read-button-area a").AddClass("fade");
            }
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
            CurrentPage = CurrentPage; // force update of slider position
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
            if (!inTransition)
            {
                ToggleInfo();
            }
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
            if (manga.nextId == -1)
            {
                jQuery.Select("#read-next-btn").Hide();
            }
            else
            {
                jQuery.Select("#read-next-btn").Show();
            }

            Show(delegate
            {
                CurrentPage = -1;
                InitializeRead();
            });
        }

        private void InitializeRead()
        {
            RemoveAllPages(delegate
            {
                UnloadAllPages();
                attachedObject.Height(jQuery.Window.GetHeight());
                oldWidth = attachedObject.GetWidth();

                if (CurrentPage == -1)
                {
                    int cp;

                    if (readNext)
                    {
                        cp = 0;
                    }
                    else
                    {
                        cp = Settings.GetCurrentPage(manga.id);
                        if (!Number.IsFinite(cp) || cp < 0 || cp >= manga.pages)
                        {
                            cp = 0;
                        }
                    }

                    CurrentPage = manga.ltr ? manga.pages - cp - 1 : cp;
                }

                readNext = true;

                NavigateTo(CurrentPage);
            });
        }

        private void NavigateTo(int page)
        {
            if (page < 0 || page >= manga.pages)
            {
                return;
            }

            RemoveAllPages(delegate
            {
                CurrentPage = page;

                LoadPages(page);
                RefreshMangaArea();
            });
        }

        private void RemoveAllPages(Action callback)
        {
            Action removeAction = delegate
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

                mangaArea.RemoveClass("fade");
                inTransition = false;
                callback();
            };

            if (Settings.UseAnimation)
            {
                inTransition = true;
                Utility.OnTransitionEnd(mangaArea.AddClass("fade"), removeAction);
            }
            else
            {
                removeAction();
            }
        }

        [AlternateSignature]
        private extern void RefreshMangaArea();
        private void RefreshMangaArea(bool force)
        {
            if (Script.IsNullOrUndefined(force)) force = true;
            if (inTransition || (!force && !Script.IsNullOrUndefined(lastRefreshMangaArea) && Date.Now - lastRefreshMangaArea <= RefreshMangaAreaInterval))
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
            int viewCenter = Math.Round(attachedObject.GetWidth() / 2);
            foreach (int page in insertedPages.Keys)
            {
                int left = Offset + insertedPages[page].Offset;
                int right = left + insertedPages[page].Width;
                if (left <= viewCenter && right >= viewCenter)
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
            if (LoadingPage)
            {
                return;
            }

            if (loadQueue.Count < 1)
            {
                return;
            }

            MangaPage page = loadQueue.Dequeue();
            LoadingPage = true;
            page.Load(
                delegate
                {
                    LoadingPage = false;
                    LoadNextPage();
                    RefreshMangaArea();
                },
                delegate
                {
                    Exit();
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

            LoadingPage = false;
        }

        private MangaPage GetMangaPage(int page)
        {
            if (loadedPages.ContainsKey(page))
            {
                return loadedPages[page];
            }
            else
            {
                return loadedPages[page] = new MangaPage(manga.id, page, attachedObject.GetWidth(), attachedObject.GetHeight(), manga.ltr, manga.pages);
            }
        }

        private MangaPage GetMangaPageFromClientX(int x)
        {
            foreach (int key in insertedPages.Keys)
            {
                int combinedOffset = Offset + insertedPages[key].Offset;
                if (combinedOffset < x && combinedOffset + insertedPages[key].Width > x)
                {
                    return insertedPages[key];
                }
            }

            return null;
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
                else if (manga.ltr)
                {
                    Offset = 0;
                }
                else
                {
                    Offset = jQuery.Window.GetWidth() - page.Width;
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
            if (!inTransition)
            {
                if (e.Type == "touch_start")
                {
                    inTouchHandler = true;
                    touchInitialOffset = Offset;
                    touchInitialXPosition = e.ClientX;

                    if (Settings.DisplayType == 2 && (currentVerticalScrollPage = GetMangaPageFromClientX(touchInitialXPosition)) != null && currentVerticalScrollPage.Height > attachedObject.GetHeight())
                    {
                        touchScrollDirection = 0;
                        touchInitialOffsetY = currentVerticalScrollPage.OffsetY;
                        touchInitialYPosition = e.ClientY;
                    }
                    else
                    {
                        touchScrollDirection = 1;
                    }
                }

                if (inTouchHandler)
                {
                    inTouchHandler = e.Type != "touch_end";

                    if (touchScrollDirection == 0)
                    {
                        if (Math.Abs(e.ClientX - touchInitialXPosition) > 35)
                        {
                            touchScrollDirection = 1;
                        }
                        else if (Math.Abs(e.ClientY - touchInitialYPosition) > 35)
                        {
                            touchScrollDirection = 2;

                            // try to snap to current page
                            if (Settings.UseAnimation)
                            {
                                mangaArea.AddClass("navigate");
                                Offset = -currentVerticalScrollPage.Offset;
                                Utility.OnTransitionEnd(
                                    mangaArea,
                                    delegate
                                    {
                                        mangaArea.RemoveClass("navigate");
                                    });
                            }
                            else
                            {
                                Offset = -currentVerticalScrollPage.Offset;
                            }
                        }
                    }

                    if (touchScrollDirection != 2)
                    {
                        MangaPage firstPage;
                        int targetOffset = e.ClientX - touchInitialXPosition + touchInitialOffset;
                        int minOffset = attachedObject.GetWidth() / 2 - (firstPage = GetMangaPage(pagesHead)).Offset - firstPage.Width;
                        int maxOffset = attachedObject.GetWidth() / 2 - GetMangaPage(pagesTail).Offset;
                        Offset = targetOffset < minOffset ? minOffset : targetOffset > maxOffset ? maxOffset : targetOffset;
                        RefreshMangaArea(false);

                        if (Settings.UseAnimation)
                        {
                            int index = 0;

                            if (e.Type == "touch_start")
                            {
                                lastTouchTime = new List<Date>();
                                lastTouchOffset = new List<int>();
                                lastTouchTime[0] = new Date();
                                lastTouchOffset[0] = Offset;
                            }
                            else
                            {
                                Date currentTime = new Date();
                                index = lastTouchOffset[lastTouchOffset.Count - 1] == Offset && currentTime - lastTouchTime[lastTouchOffset.Count - 1] < 100 ? lastTouchOffset.Count - 1 : lastTouchOffset.Count;
                                lastTouchTime[index] = currentTime;
                                lastTouchOffset[index] = Offset;
                            }

                            if (e.Type == "touch_end" && index > 0)
                            {
                                int dx = lastTouchOffset[index] - lastTouchOffset[index - 1];
                                int dt = lastTouchTime[index] - lastTouchTime[index - 1] + 1;
                                int distance = Math.Round(dx * 100 / dt);

                                lastTouchOffset = (List<int>)(object)(lastTouchTime = null);

                                if (distance != 0)
                                {
                                    inTransition = true;
                                    mangaArea.AddClass("inertia");
                                    Offset += distance;
                                    Utility.OnTransitionEnd(
                                        mangaArea,
                                        delegate
                                        {
                                            inTransition = false;
                                            mangaArea.RemoveClass("inertia");
                                            RefreshMangaArea();

                                            minOffset = attachedObject.GetWidth() / 2 - (firstPage = GetMangaPage(pagesHead)).Offset - firstPage.Width;
                                            maxOffset = attachedObject.GetWidth() / 2 - GetMangaPage(pagesTail).Offset;

                                            if (Offset < minOffset || Offset > maxOffset)
                                            {
                                                inTransition = true;
                                                mangaArea.AddClass("inertia-bounce");
                                                Offset = Offset < minOffset ? minOffset : maxOffset;
                                                Utility.OnTransitionEnd(
                                                    mangaArea,
                                                    delegate
                                                    {
                                                        inTransition = false;
                                                        mangaArea.RemoveClass("inertia-bounce");
                                                        RefreshMangaArea();
                                                    });
                                            }
                                        });
                                }
                            }
                        }
                    }

                    if (touchScrollDirection != 1)
                    {
                        int targetOffset = e.ClientY - touchInitialYPosition + touchInitialOffsetY;
                        int minOffset = attachedObject.GetHeight() - currentVerticalScrollPage.Height;
                        int maxOffset = 0;
                        currentVerticalScrollPage.OffsetY = targetOffset < minOffset ? minOffset : targetOffset > maxOffset ? maxOffset : targetOffset;

                        if (Settings.UseAnimation)
                        {
                            int index = 0;

                            if (e.Type == "touch_start")
                            {
                                lastTouchTimeY = new List<Date>();
                                lastTouchOffsetY = new List<int>();
                                lastTouchTimeY[0] = new Date();
                                lastTouchOffsetY[0] = currentVerticalScrollPage.OffsetY;
                            }
                            else
                            {
                                Date currentTime = new Date();
                                index = lastTouchOffsetY[lastTouchOffsetY.Count - 1] == currentVerticalScrollPage.OffsetY && currentTime - lastTouchTimeY[lastTouchOffsetY.Count - 1] < 100 ? lastTouchOffsetY.Count - 1 : lastTouchOffsetY.Count;
                                lastTouchTimeY[index] = currentTime;
                                lastTouchOffsetY[index] = currentVerticalScrollPage.OffsetY;
                            }

                            if (e.Type == "touch_end" && index > 0)
                            {
                                int dy = lastTouchOffsetY[index] - lastTouchOffsetY[index - 1];
                                int dt = lastTouchTimeY[index] - lastTouchTimeY[index - 1] + 1;
                                int distance = Math.Round(dy * 100 / dt);

                                lastTouchOffsetY = (List<int>)(object)(lastTouchTimeY = null);

                                if (distance != 0)
                                {
                                    inTransition = true;
                                    currentVerticalScrollPage.AttachedObject.AddClass("inertia");
                                    currentVerticalScrollPage.OffsetY += distance;
                                    Utility.OnTransitionEnd(
                                        currentVerticalScrollPage.AttachedObject,
                                        delegate
                                        {
                                            inTransition = false;
                                            currentVerticalScrollPage.AttachedObject.RemoveClass("inertia");

                                            if (currentVerticalScrollPage.OffsetY < minOffset || currentVerticalScrollPage.OffsetY > maxOffset)
                                            {
                                                inTransition = true;
                                                currentVerticalScrollPage.AttachedObject.AddClass("inertia-bounce");
                                                currentVerticalScrollPage.OffsetY = currentVerticalScrollPage.OffsetY < minOffset ? minOffset : maxOffset;
                                                Utility.OnTransitionEnd(
                                                    currentVerticalScrollPage.AttachedObject,
                                                    delegate
                                                    {
                                                        inTransition = false;
                                                        currentVerticalScrollPage.AttachedObject.RemoveClass("inertia-bounce");
                                                    });
                                            }
                                        });
                                }
                            }
                        }
                    }
                }
            }
        }

        [AlternateSignature]
        private extern void SliderTouchHandler(jQueryEvent e);
        private void SliderTouchHandler(jQueryTouchEvent e)
        {
            if (!inTransition)
            {
                if (e.Type == "touch_start")
                {
                    inSliderTouchHandler = true;
                    sliderTouchInitialOffset = SliderOffset;
                    sliderTouchInitialXPosition = e.ClientX;
                }
                else if (inSliderTouchHandler)
                {
                    int targetOffset = e.ClientX - sliderTouchInitialXPosition + sliderTouchInitialOffset;
                    int maxOffset = sliderHandle.Parent().GetInnerWidth() - sliderHandle.GetOuterWidth();
                    targetOffset = targetOffset < 0 ? 0 : targetOffset > maxOffset ? maxOffset : targetOffset;
                    CurrentPage = manga.pages - 1 - Math.Floor(targetOffset / (maxOffset + 1) * manga.pages);
                    SliderOffset = targetOffset;
                    if (e.Type == "touch_end")
                    {
                        inSliderTouchHandler = false;
                        NavigateTo(CurrentPage);
                    }
                }
            }
        }

        private void ExitButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            if (!inTransition)
            {
                Exit();
            }
        }

        private void Exit()
        {
            readNext = false;
            HideInfo();
            RemoveAllPages(delegate
            {
                UnloadAllPages();
                MangasModule.Instance.Show(null);
            });
        }

        private void NextButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            if (!inTransition && !sendingReadReqeust && manga.nextId != -1)
            {
                sendingReadReqeust = true;
                MangaReadRequest request = new MangaReadRequest();
                request.id = manga.nextId;
                request.nextId = MangasModule.Instance.GetNextMangaId(manga.nextId);
                Request.Send(request, ReadRequestSuccess, ReadRequestFailure);
            }
        }

        private void DirectionButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (!inTransition && !sendingDirectionReqeust)
            {
                sendingDirectionReqeust = true;
                MangaDirectionRequest request = new MangaDirectionRequest();
                request.id = manga.id;
                Request.Send(request, DirectionRequestSuccess, DirectionRequestFailure);
            }
        }

        [AlternateSignature]
        private extern void ReadRequestSuccess(JsonResponse response);
        private void ReadRequestSuccess(MangaReadResponse response)
        {
            sendingReadReqeust = false;
            ReadManga(response);
        }

        private void ReadRequestFailure(Exception error)
        {
            sendingReadReqeust = false;
            ErrorModal.ShowError(error.ToString());
        }

        private void DirectionRequestSuccess(JsonResponse response)
        {
            sendingDirectionReqeust = false;
            manga.ltr = !manga.ltr;
            CurrentPage = manga.pages - CurrentPage - 1;
            InitializeRead();
        }

        private void DirectionRequestFailure(Exception error)
        {
            sendingDirectionReqeust = false;
            ErrorModal.ShowError(error.ToString());
        }

        private void ArrowButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            if (!inTransition)
            {
                jQueryObject target = jQuery.FromElement(e.Target);
                while (!target.Is("a"))
                {
                    target = target.Parent();
                }

                NavigateLeftOrRight(target.GetAttribute("data-direction") == "left");
            }
        }

        private void NavigateForwardOrBackward(bool backward)
        {
            NavigateLeftOrRight(!backward ^ manga.ltr);
        }

        private void NavigateLeftOrRight(bool left)
        {
            int? distance = null;

            foreach (int page in insertedPages.Keys)
            {
                MangaPage mangaPage = insertedPages[page];
                if (left)
                {
                    int pagePos = Offset + mangaPage.Offset;
                    if (pagePos < 0 && (Script.IsNullOrUndefined(distance) || pagePos > distance.Value))
                    {
                        distance = pagePos;
                    }
                }
                else
                {
                    int pagePos = Offset + mangaPage.Offset + mangaPage.Width - attachedObject.GetWidth();
                    if (pagePos > 0 && (Script.IsNullOrUndefined(distance) || pagePos < distance.Value))
                    {
                        distance = pagePos;
                    }
                }
            }

            if (!Script.IsNullOrUndefined(distance))
            {
                if (Math.Abs(distance.Value) > attachedObject.GetWidth())
                {
                    distance = attachedObject.GetWidth() * (distance.Value > 0 ? 1 : -1);
                }

                if (!Settings.UseAnimation)
                {
                    Offset -= distance.Value;
                    RefreshMangaArea();
                    return;
                }

                inTransition = true;
                mangaArea.AddClass("navigate");
                Offset -= distance.Value;
                Utility.OnTransitionEnd(
                    mangaArea,
                    delegate
                    {
                        inTransition = false;
                        mangaArea.RemoveClass("navigate");
                        RefreshMangaArea();
                    });
            }
        }

        private void OnKeyUp(jQueryEvent keyEvent)
        {
            if (attachedObject.Is(":visible") && !inTransition)
            {
                if (Script.IsNullOrUndefined(keyEvent))
                {
                    keyEvent = (jQueryEvent)(object)Window.Event;
                }

                ElementEvent evt = (ElementEvent)(object)keyEvent;

                if (evt.KeyCode == 33)
                {
                    // 33 == page up
                    NavigateForwardOrBackward(true); // previous page
                    evt.PreventDefault();
                }
                else if (evt.KeyCode == 34)
                {
                    // 34 == page down
                    NavigateForwardOrBackward(false); // next page
                    evt.PreventDefault();
                }
                else if (evt.KeyCode == 37)
                {
                    // 37 == left arrow
                    NavigateLeftOrRight(true); // left
                    evt.PreventDefault();
                }
                else if (evt.KeyCode == 39)
                {
                    // 39 == right arrow
                    NavigateLeftOrRight(false); // right
                    evt.PreventDefault();
                }
                else if (evt.KeyCode == 27)
                {
                    // 27 == Escape
                    Exit();
                    evt.PreventDefault();
                }
            }
        }

        private void MouseWheelHandler(jQueryEvent e)
        {
            if (attachedObject.Is(":visible") && !inTransition)
            {
                e.PreventDefault();
                MouseWheelEvent wheelEvent = (MouseWheelEvent)(object)((Dictionary<string, object>)(object)e)["originalEvent"];

                int delta = 0;
                if (!Script.IsNullOrUndefined(wheelEvent.WheelDelta))
                {
                    if (!Script.IsNullOrUndefined(wheelEvent.WheelDeltaX) && wheelEvent.WheelDeltaX != 0)
                    {
                        delta = wheelEvent.WheelDeltaX / -120;
                        hasHorizonalMouseWheelScroll = true;
                    }
                    else if (!hasHorizonalMouseWheelScroll)
                    {
                        delta = wheelEvent.WheelDelta / 120;
                    }
                }
                else if (!Script.IsNullOrUndefined(wheelEvent.Detail))
                {
                    int detail = int.Parse(wheelEvent.Detail, 10);
                    if (detail != 0)
                    {
                        delta = detail / -3;
                        if (wheelEvent.Axis == wheelEvent.HorizontalAxis)
                        {
                            delta *= -1;
                            hasHorizonalMouseWheelScroll = true;
                        }
                        else if (hasHorizonalMouseWheelScroll)
                        {
                            delta = 0;
                        }
                    }
                }

                if (delta != 0)
                {
                    MangaPage firstPage;
                    int targetOffset = Offset - delta * 100 * (manga.ltr ? -1 : 1);
                    int minOffset = attachedObject.GetWidth() / 2 - (firstPage = GetMangaPage(pagesHead)).Offset - firstPage.Width;
                    int maxOffset = attachedObject.GetWidth() / 2 - GetMangaPage(pagesTail).Offset;
                    Offset = targetOffset < minOffset ? minOffset : targetOffset > maxOffset ? maxOffset : targetOffset;
                    RefreshMangaArea(false);
                }
            }
        }

        private void OnResize(jQueryEvent e)
        {
            if (attachedObject.Is(":visible"))
            {
                resizeTimeout.Start(
                    delegate
                    {
                        if (attachedObject.Is(":visible"))
                        {
                            if (Settings.DisplayType == 0 && attachedObject.GetHeight() == jQuery.Window.GetHeight())
                            {
                                return;
                            }

                            if (Settings.DisplayType == 1 && attachedObject.GetHeight() == jQuery.Window.GetHeight() && oldWidth == jQuery.Window.GetWidth())
                            {
                                return;
                            }

                            InitializeRead();
                        }
                    },
                    1000);
            }
        }
    }
}
