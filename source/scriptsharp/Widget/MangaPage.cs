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
        private bool ltr;
        private int totalPages;
        private Action onload;
        private Action onfailure;
        private jQueryObject imageObject;
        private jQueryObject imagePart1Object;
        private jQueryObject imagePart2Object;
        private MangaPageRequest pageRequest;
        private int pageRequestDelay;
        private bool loaded;
        private bool loading;
        private bool unloaded;
        private int _offset;
        private int _offsetY;

        public int Offset
        {
            get
            {
                return _offset;
            }
            private set
            {
                _offset = Math.Round(value);
                UpdateCssTransform();
            }
        }

        public int OffsetY
        {
            get
            {
                return _offsetY;
            }
            set
            {
                _offsetY = Math.Round(value);
                UpdateCssTransform();
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

        private ImageElement imageElementPart1
        {
            get
            {
                return imagePart1Object == null ? null : (ImageElement)imagePart1Object.GetElement(0);
            }
        }

        private ImageElement imageElementPart2
        {
            get
            {
                return imagePart2Object == null ? null : (ImageElement)imagePart2Object.GetElement(0);
            }
        }

        public jQueryObject AttachedObject
        {
            get
            {
                return imageObject;
            }
        }

        public MangaPage(int mangaId, int page, int width, int height, bool ltr, int totalPages)
        {
            this.mangaId = mangaId;
            this.page = page;
            this.width = width - 2;
            this.height = height;
            this.ltr = ltr;
            this.totalPages = totalPages;
            unloaded = loading = loaded = false;
        }

        public void Load(Action onload, Action onfailure)
        {
            if (loaded)
            {
                onload();
                return;
            }

            this.onload = onload;
            this.onfailure = onfailure;

            loading = true;

            pageRequest = new MangaPageRequest();
            pageRequest.id = mangaId;
            pageRequest.page = ltr ? totalPages - page - 1 : page;
            pageRequest.width = Settings.DisplayType == 0 ? 0 : (int)Math.Round(width * Environment.PixelRatio); // Fit Height?
            pageRequest.height = Settings.DisplayType == 2 ? 0 : (int)Math.Round(height * Environment.PixelRatio); // Fit Width?
            pageRequest.dimensions = Environment.IsiOS && Settings.FixAutoDownscale;
            pageRequest.part = 0;

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
                        // check iOS downscale issue
                        if (pageRequest.dimensions
                            && (pageRequest.width == 0 || (imageElement.Width < pageRequest.width && imageElement.Width < response.dimensions[0]))
                            && (pageRequest.height == 0 || (imageElement.Height < pageRequest.height && imageElement.Height < response.dimensions[1])))
                        {
                            imageObject.Unbind("load").Attribute("src", "");
                            imageObject = null;
                            LoadPart1();
                            return;
                        }

                        LoadFinish();
                    }).Attribute("src", response.url);
            }
            else if (response.status == 1)
            {
                Window.SetTimeout(
                    delegate
                    {
                        Request.Send(pageRequest, MangaPageRequestSucess);
                    },
                    pageRequestDelay = pageRequestDelay + 1000);
            }
            else
            {
                ErrorModal.ShowError(Strings.Get("MangaNotAvailable"));
                onfailure();
            }
        }

        private void LoadPart1()
        {
            pageRequestDelay = 0;
            pageRequest.dimensions = false;
            pageRequest.part = 1;
            Request.Send(pageRequest, MangaPagePart1RequestSucess);
        }

        [AlternateSignature]
        private extern void MangaPagePart1RequestSucess(JsonResponse response);
        private void MangaPagePart1RequestSucess(MangaImageResponse response)
        {
            if (unloaded)
            {
                return;
            }

            if (response.status == 0)
            {
                imagePart1Object = jQuery.FromHtml("<img>").Bind(
                    "load",
                    delegate(jQueryEvent e)
                    {
                        LoadPart2();
                    }).Attribute("src", response.url);
            }
            else if (response.status == 1)
            {
                Window.SetTimeout(
                    delegate
                    {
                        Request.Send(pageRequest, MangaPagePart1RequestSucess);
                    },
                    pageRequestDelay = pageRequestDelay * 2 + 1000);
            }
            else
            {
                ErrorModal.ShowError(Strings.Get("MangaNotAvailable"));
                onfailure();
            }
        }

        private void LoadPart2()
        {
            pageRequestDelay = 0;
            pageRequest.part = 2;
            Request.Send(pageRequest, MangaPagePart2RequestSucess);
        }

        [AlternateSignature]
        private extern void MangaPagePart2RequestSucess(JsonResponse response);
        private void MangaPagePart2RequestSucess(MangaImageResponse response)
        {
            if (unloaded)
            {
                return;
            }

            if (response.status == 0)
            {
                imagePart2Object = jQuery.FromHtml("<img>").Bind(
                    "load",
                    delegate(jQueryEvent e)
                    {
                        LoadFinish();
                    }).Attribute("src", response.url);
            }
            else if (response.status == 1)
            {
                Window.SetTimeout(
                    delegate
                    {
                        Request.Send(pageRequest, MangaPagePart2RequestSucess);
                    },
                    pageRequestDelay = pageRequestDelay * 2 + 1000);
            }
            else
            {
                ErrorModal.ShowError(Strings.Get("MangaNotAvailable"));
                onfailure();
            }
        }

        private void LoadFinish()
        {
            if (imageObject != null && imageObject.Is("img"))
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
            }
            else
            {
                imageObject = jQuery.FromHtml("<div></div>").Append(imagePart1Object).Append(imagePart2Object);

                if (Settings.DisplayType == 0)
                {
                    imagePart1Object.Height(height);
                    imagePart2Object.Height(height);
                    imageObject.Height(height).Width(imageElementPart1.Width / imageElementPart1.Height * height * 2);
                }
                else if (Settings.DisplayType == 2)
                {
                    int halfWidth = Math.Round(width / 2);
                    imagePart1Object.Width(halfWidth);
                    imagePart2Object.Width(halfWidth);
                    imageObject.Width(halfWidth * 2);
                }
                else
                {
                    double widthFactor = width / imageElementPart1.Width / 2;
                    double heightFactor = height / imageElementPart1.Height;
                    double factor = widthFactor < heightFactor ? widthFactor : heightFactor;
                    int halfWidth = Math.Round(imageElementPart1.Width * factor);
                    int sHeight = Math.Round(imageElementPart1.Height * factor);
                    imagePart1Object.Width(halfWidth).Height(sHeight);
                    imagePart2Object.Width(halfWidth).Height(sHeight);
                    imageObject.Width(halfWidth * 2).Height(sHeight);
                }
            }

            loading = false;
            loaded = true;
            onload();
        }

        public void AppendTo(jQueryObject mangaArea, int offset, int sign, MangaPage otherPage)
        {
            if (Settings.UseAnimation)
            {
                imageObject.AddClass("fade");
                Window.SetTimeout(
                    delegate
                    {
                        imageObject.AddClass("in");
                    },
                    1);
            }

            imageObject.AddClass("read-manga-page").AppendTo(mangaArea);
            Offset = offset + sign * otherPage.Width;
            OffsetY = Height > mangaArea.GetHeight() ? 0 : ((mangaArea.GetHeight() - Height) / 2);

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

        public int Height
        {
            get
            {
                return imageObject == null ? 0 : imageObject.GetOuterHeight();
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

            if (imagePart1Object != null && imagePart1Object.Is("img"))
            {
                imagePart1Object.Unbind("load").Attribute("src", "");
                imagePart1Object = null;
            }

            if (imagePart2Object != null && imagePart2Object.Is("img"))
            {
                imagePart2Object.Unbind("load").Attribute("src", "");
                imagePart1Object = null;
            }

            if (loading && loadNext)
            {
                loading = false;
                onload();
            }
        }

        private void UpdateCssTransform()
        {
            if (Environment.IsIE9OrLower)
            {
                string cssValue = "translate(" + _offset + "px," + _offsetY + "px)";
                imageObject.CSS("transform", cssValue).CSS("-ms-transform", cssValue);
            }
            else
            {
                string cssValue = "translate3d(" + _offset + "px," + _offsetY + "px,0)";
                imageObject.CSS("transform", cssValue).CSS("-ms-transform", cssValue).CSS("-moz-transform", cssValue).CSS("-webkit-transform", cssValue).CSS("-o-transform", cssValue);
            }
        }
    }
}
