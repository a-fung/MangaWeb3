// MangaListItem.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Widget
{
    public class MangaListItem
    {
        private jQueryObject attachedObject;
        private MangaListItemCoverRequest coverRequest;

        public MangaListItem(jQueryObject parent, MangaListItemJson data)
        {
            attachedObject = Template.Get("client", "mangas-list-item", true).AppendTo(parent);
            jQuery.Select(".mangas-list-item-title", attachedObject).Text(data.title);
            jQuery.Select(".mangas-list-item-pages", attachedObject).Text(data.pages.ToString());

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

            coverRequest = new MangaListItemCoverRequest();
            coverRequest.id = data.id;
            Request.Send(coverRequest, CoverRequestSuccess);
        }

        [AlternateSignature]
        private extern void CoverRequestSuccess(JsonResponse response);
        private void CoverRequestSuccess(MangaImageResponse response)
        {
            if (response.status == 0)
            {
                jQuery.Select(".mangas-list-item-thumbnail", attachedObject).Attribute("src", response.url);
            }
            else if (response.status == 1)
            {
                Window.SetTimeout(
                    delegate
                    {
                        Request.Send(coverRequest, CoverRequestSuccess);
                    },
                    1000);
            }
            else
            {
            }
        }
    }
}
