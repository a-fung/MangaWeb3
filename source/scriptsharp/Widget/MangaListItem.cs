// MangaListItem.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Client.Module;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Widget
{
    public class MangaListItem
    {
        private jQueryObject attachedObject;
        private MangaListItemCoverRequest coverRequest;
        private bool coverLoaded;
        private int coverRequestDelay;
        private int nextMangaId;
        private MangaListItemJson data;

        private static bool sendingReadReqeust = false;

        private static Queue<MangaListItem> loadQueue = new Queue<MangaListItem>();
        private static SelfLimitingTimeout loadNextTimeout = new SelfLimitingTimeout();
        private static Dictionary<int, string> coverCache = new Dictionary<int, string>();

        public MangaListItem(jQueryObject parent, MangaListItemJson data, int nextMangaId)
        {
            attachedObject = Template.Get("client", "mangas-list-item", true).AppendTo(parent);
            jQuery.Select(".mangas-list-item-title", attachedObject).Text(data.title);
            jQuery.Select(".mangas-list-item-pages", attachedObject).Text(data.pages.ToString());
            coverLoaded = false;
            coverRequestDelay = 500;
            this.nextMangaId = nextMangaId;
            this.data = data;

            double size = data.size;
            string unit;
            if (size > 1000)
            {
                size /= 1024;

                if (size > 1000)
                {
                    size /= 1024;

                    if (size > 1000)
                    {
                        size /= 1024;
                        unit = Strings.Get("GigaBytes");
                    }
                    else
                    {
                        unit = Strings.Get("MegaBytes");
                    }
                }
                else
                {
                    unit = Strings.Get("KiloBytes");
                }
            }
            else
            {
                unit = Strings.Get("Bytes");
            }

            if (size < 10)
            {
                size = Math.Floor(size * 100) / 100;
            }
            else if (size < 100)
            {
                size = Math.Floor(size * 10) / 10;
            }
            else
            {
                size = Math.Floor(size);
            }

            jQuery.Select(".mangas-list-item-size", attachedObject).Text(size.ToString());
            jQuery.Select(".mangas-list-item-size-unit", attachedObject).Text(unit);
            jQuery.Select(".mangas-list-item-details-btn", attachedObject).AddClass("disabled").Click(DetailsButtonClicked);
            jQuery.Select(".mangas-list-item-thumbnail-link", attachedObject).Click(CoverClicked);

            if (!Settings.UseAnimation)
            {
                TryLoadFromCache();
                return;
            }

            attachedObject.AddClass("fade");
            Window.SetTimeout(
                delegate
                {
                    Utility.OnTransitionEnd(
                        attachedObject.AddClass("in"),
                        delegate
                        {
                            TryLoadFromCache();
                        });
                },
                1);
        }

        private void TryLoadFromCache()
        {
            if (coverCache.ContainsKey(data.id))
            {
                LoadCover(coverCache[data.id]);
            }
            else
            {
                loadQueue.Enqueue(this);
                LoadNextItem();
            }
        }

        public bool Load()
        {
            if (attachedObject.Is(":visible"))
            {
                coverRequest = new MangaListItemCoverRequest();
                coverRequest.id = data.id;
                Request.Send(coverRequest, CoverRequestSuccess);

                return true;
            }

            return false;
        }

        [AlternateSignature]
        private extern void CoverRequestSuccess(JsonResponse response);
        private void CoverRequestSuccess(MangaImageResponse response)
        {
            if (response.status == 0)
            {
                coverCache[data.id] = response.url;
                if (attachedObject.Is(":visible"))
                {
                    LoadCover(coverCache[data.id]);
                }
            }
            else if (response.status == 1 && attachedObject.Is(":visible"))
            {
                Window.SetTimeout(
                    delegate
                    {
                        Request.Send(coverRequest, CoverRequestSuccess);
                    },
                    coverRequestDelay = Math.Round(coverRequestDelay * 1.1));
            }
            else if (response.status == 2)
            {
                attachedObject.Remove();
            }
        }

        private void LoadCover(string coverUrl)
        {
            if (!Settings.UseAnimation)
            {
                jQuery.Select(".mangas-list-item-thumbnail", attachedObject).Attribute("src", coverUrl).One(
                    "load",
                    delegate(jQueryEvent e)
                    {
                        coverLoaded = true;
                        jQuery.Select(".mangas-list-item-details-btn", attachedObject).RemoveClass("disabled");
                    });
                return;
            }

            jQueryObject wrap = jQuery.Select(".mangas-list-item-thumbnail-wrap", attachedObject);
            wrap.Height(wrap.GetHeight());
            jQueryObject placeholderThumbnail = jQuery.Select(".mangas-list-item-thumbnail", attachedObject);
            jQueryObject thumbnail = placeholderThumbnail.Clone();
            thumbnail.Attribute("src", coverUrl).One(
                "load",
                delegate(jQueryEvent e)
                {
                    if (!attachedObject.Is(":visible"))
                    {
                        thumbnail.Attribute("src", "");
                        return;
                    }

                    thumbnail.AddClass("fade");
                    Utility.OnTransitionEnd(
                        placeholderThumbnail.AddClass("fade"),
                        delegate
                        {
                            if (!attachedObject.Is(":visible"))
                            {
                                thumbnail.Attribute("src", "");
                                return;
                            }

                            placeholderThumbnail.After(thumbnail);
                            placeholderThumbnail.Remove();

                            Action wrapChangeHeight = null;
                            wrapChangeHeight = delegate
                            {
                                if (thumbnail.GetHeight() < 1)
                                {
                                    Window.SetTimeout(wrapChangeHeight, 100);
                                    return;
                                }

                                Utility.OnTransitionEnd(
                                    wrap.AddClass("height-transition").Height(thumbnail.GetHeight()),
                                    delegate
                                    {
                                        Utility.OnTransitionEnd(
                                            thumbnail.AddClass("in"),
                                            delegate
                                            {
                                                thumbnail.RemoveClass("fade in");
                                                wrap.RemoveClass("height-transition").CSS("height", "");
                                                coverLoaded = true;
                                                jQuery.Select(".mangas-list-item-details-btn", attachedObject).RemoveClass("disabled");
                                            });
                                    });
                            };

                            wrapChangeHeight();
                        });
                });
        }

        private void DetailsButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            if (coverLoaded)
            {
                jQueryObject buttonP = jQuery.Select(".mangas-list-item-details-p", attachedObject);

                Action removeButtonAndSendRequest = delegate
                {
                    buttonP.Remove();

                    MangaListItemDetailsRequest request = new MangaListItemDetailsRequest();
                    request.id = data.id;
                    Request.Send(request, DetailsRequestSuccess);
                };

                if (Settings.UseAnimation)
                {
                    jQuery.Select(".mangas-list-item-frame", attachedObject).AddClass("height-transition-suppress").Height(jQuery.Select(".mangas-list-item-inner", attachedObject).GetHeight());
                    Utility.OnTransitionEnd(buttonP.AddClass("fade"), removeButtonAndSendRequest);
                }
                else
                {
                    removeButtonAndSendRequest();
                }
            }
        }

        [AlternateSignature]
        private extern void DetailsRequestSuccess(JsonResponse response);
        private void DetailsRequestSuccess(MangaListItemDetailsResponse response)
        {
            bool fadeIn = true;
            jQueryObject detailsDiv = Template.Get("client", "mangas-list-item-details", true).AppendTo(jQuery.Select(".caption", attachedObject));

            if (Settings.UseAnimation)
            {
                detailsDiv.AddClass("fade");
            }

            if (String.IsNullOrEmpty(response.author))
            {
                jQuery.Select(".mangas-list-item-details-author-p", detailsDiv).Remove();
            }
            else
            {
                jQuery.Select(".mangas-list-item-details-author", detailsDiv).Text(response.author);
            }

            if (String.IsNullOrEmpty(response.series))
            {
                jQuery.Select(".mangas-list-item-details-series-p", detailsDiv).Remove();
            }
            else
            {
                jQuery.Select(".mangas-list-item-details-series", detailsDiv).Text(response.series);
            }

            if (response.volume < 0)
            {
                jQuery.Select(".mangas-list-item-details-volume-p", detailsDiv).Remove();
            }
            else
            {
                jQuery.Select(".mangas-list-item-details-volume", detailsDiv).Text(response.volume.ToString());
            }

            if (response.year < 0)
            {
                jQuery.Select(".mangas-list-item-details-year-p", detailsDiv).Remove();
            }
            else
            {
                jQuery.Select(".mangas-list-item-details-year", detailsDiv).Text(response.year.ToString());
            }

            if (String.IsNullOrEmpty(response.publisher))
            {
                jQuery.Select(".mangas-list-item-details-publisher-p", detailsDiv).Remove();
            }
            else
            {
                jQuery.Select(".mangas-list-item-details-publisher", detailsDiv).Text(response.publisher);
            }

            if (response.tags == null || response.tags.Length == 0)
            {
                jQuery.Select(".mangas-list-item-details-tags-p", detailsDiv).Remove();
            }
            else
            {
                for (int i = 0; i < response.tags.Length; i++)
                {
                    string tag = response.tags[i];

                    Template.Get("client", "mangas-list-item-details-tag-link", true)
                        .AppendTo(jQuery.Select(".mangas-list-item-details-tags", detailsDiv))
                        .Text(tag)
                        .Attribute("data-tag", tag)
                        .Click(TagClicked);

                    if (i < response.tags.Length - 1)
                    {
                        jQuery.Select(".mangas-list-item-details-tags", detailsDiv).Append(", ");
                    }
                }
            }

            if (detailsDiv.Children().Length == 0)
            {
                detailsDiv.Remove();
                fadeIn = false;
            }

            if (!Settings.UseAnimation)
            {
                return;
            }

            Action removeFrameHeight = delegate
            {
                jQuery.Select(".mangas-list-item-frame", attachedObject).AddClass("height-transition-suppress").CSS("height", "");
                Window.SetTimeout(
                    delegate
                    {
                        jQuery.Select(".mangas-list-item-frame", attachedObject).RemoveClass("height-transition-suppress");
                    },
                    1);
            };

            Utility.OnTransitionEnd(
                jQuery.Select(".mangas-list-item-frame", attachedObject).RemoveClass("height-transition-suppress").Height(jQuery.Select(".mangas-list-item-inner", attachedObject).GetHeight()),
                delegate
                {
                    if (fadeIn)
                    {
                        Utility.OnTransitionEnd(detailsDiv.AddClass("in"), removeFrameHeight);
                    }
                    else
                    {
                        removeFrameHeight();
                    }
                });
        }

        private void TagClicked(jQueryEvent e)
        {
            e.PreventDefault();
            MangaFilter filter = new MangaFilter();
            filter.tag = jQuery.FromElement(e.Target).GetAttribute("data-tag");
            MangasModule.Instance.Refresh(filter);
        }

        private void CoverClicked(jQueryEvent e)
        {
            e.PreventDefault();
            if (coverLoaded && !sendingReadReqeust)
            {
                sendingReadReqeust = true;

                MangaReadRequest request = new MangaReadRequest();
                request.id = data.id;
                request.nextId = nextMangaId;
                Request.Send(request, ReadRequestSuccess, ReadRequestFailure);
            }
        }

        [AlternateSignature]
        private extern void ReadRequestSuccess(JsonResponse response);
        private void ReadRequestSuccess(MangaReadResponse response)
        {
            sendingReadReqeust = false;
            ReadModule.ReadManga(response);
        }

        private void ReadRequestFailure(Exception error)
        {
            sendingReadReqeust = false;
            ErrorModal.ShowError(error.ToString());
        }

        private static void LoadNextItem()
        {
            loadNextTimeout.Start(
                delegate
                {
                    if (loadQueue.Count < 1)
                    {
                        return;
                    }

                    MangaListItem item;
                    while ((item = loadQueue.Dequeue()) != null && !item.Load())
                    {
                    }

                    LoadNextItem();
                },
                350);
        }
    }
}
