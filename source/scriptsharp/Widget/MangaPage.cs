// MangaPage.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Widget
{
    public class MangaPage
    {
        private int mangaId;
        private int page;
        private int width;
        private int height;
        private Action onload;
        private jQueryObject imageObject;
        private MangaPageRequest pageRequest;
        private int pageRequestDelay;
        private bool loaded;
        private bool loading;
        private bool unloaded;
        private int _offset;

        public int Offset
        {
            get
            {
                return _offset;
            }
            private set
            {
                _offset = Math.Round(value);
                if (jQuery.Browser.MSIE && float.Parse(jQuery.Browser.Version) < 10)
                {
                    string cssValue = "translate(" + _offset + "px,0)";
                    imageObject.CSS("transform", cssValue).CSS("-ms-transform", cssValue);
                }
                else
                {
                    string cssValue = "translate3d(" + _offset + "px,0,0)";
                    imageObject.CSS("transform", cssValue).CSS("-ms-transform", cssValue).CSS("-moz-transform", cssValue).CSS("-webkit-transform", cssValue).CSS("-o-transform", cssValue);
                }
            }
        }

        public bool Loaded
        {
            get
            {
                return loaded;
            }
        }

        public int Page
        {
            get
            {
                return page;
            }
        }

        private ImageElement imageElement
        {
            get
            {
                return imageObject == null || !imageObject.Is("img") ? null : (ImageElement)imageObject.GetElement(0);
            }
        }

        public MangaPage(int mangaId, int page, int width, int height)
        {
            this.mangaId = mangaId;
            this.page = page;
            this.width = width - 2;
            this.height = height;
            unloaded = loading = loaded = false;
        }

        public void Load(Action onload)
        {
            if (loaded)
            {
                onload();
                return;
            }

            this.onload = onload;

            loading = true;

            pageRequest = new MangaPageRequest();
            pageRequest.id = mangaId;
            pageRequest.page = page;
            pageRequest.width = Settings.DisplayType == 0 ? 0 : (int)Math.Round(width * Environment.PixelRatio); // Fit Height?
            pageRequest.height = Settings.DisplayType == 2 ? 0 : (int)Math.Round(height * Environment.PixelRatio); // Fit Width?

            Request.Send(pageRequest, MangaPageRequestSucess);
        }

        [AlternateSignature]
        private extern void MangaPageRequestSucess(JsonResponse response);
        private void MangaPageRequestSucess(MangaImageResponse response)
        {
            if (unloaded)
            {
                return;
            }

            if (response.status == 0)
            {
                imageObject = jQuery.FromHtml("<img>").Bind(
                    "load",
                    delegate(jQueryEvent e)
                    {
                        if (Settings.DisplayType == 0)
                        {
                            imageObject.Height(height);
                        }
                        else if (Settings.DisplayType == 2)
                        {
                            imageObject.Width(width);
                        }
                        else
                        {
                            double widthFactor = width / imageElement.Width;
                            double heightFactor = height / imageElement.Height;
                            double factor = widthFactor < heightFactor ? widthFactor : heightFactor;
                            imageObject
                                .Width(Math.Round(imageElement.Width * factor))
                                .Height(Math.Round(imageElement.Height * factor));
                        }

                        loading = false;
                        loaded = true;
                        onload();
                    }).Attribute("src", response.url);
            }
            else if (response.status == 1)
            {
                Window.SetTimeout(
                    delegate
                    {
                        Request.Send(pageRequest, MangaPageRequestSucess);
                    },
                    pageRequestDelay = pageRequestDelay * 2 + 1000);
            }
            else
            {
                ErrorModal.ShowError(Strings.Get("MangaNotAvailable"));
            }
        }

        public void AppendTo(jQueryObject mangaArea, int offset, int sign, MangaPage otherPage)
        {
            imageObject.AddClass("read-manga-page fade").AppendTo(mangaArea);
            Offset = offset + sign * otherPage.Width;
            Window.SetTimeout(
                delegate
                {
                    imageObject.AddClass("in");
                },
                1);
        }

        public void Remove()
        {
            imageObject.Remove();
        }

        public int Width
        {
            get
            {
                return imageObject == null ? 0 : imageObject.GetOuterWidth();
            }
        }

        [AlternateSignature]
        public extern void Unload();
        public void Unload(bool loadNext)
        {
            if (Script.IsNullOrUndefined(loadNext)) loadNext = true;
            loaded = false;
            unloaded = true;

            if (imageObject != null && imageObject.Is("img"))
            {
                imageObject.Unbind("load").Attribute("src", "");
                imageObject = null;
            }

            if (loading && loadNext)
            {
                loading = false;
                onload();
            }
        }
    }
}
