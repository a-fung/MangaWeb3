using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class MangaDirectionRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(MangaDirectionRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            MangaDirectionRequest request = Utility.ParseJson<MangaDirectionRequest>(jsonString);

            Manga manga = Manga.GetById(request.id);

            if (manga == null || !manga.ParentCollection.Accessible(ajax))
            {
                ajax.BadRequest();
                return;
            }

            manga.LeftToRight = !manga.LeftToRight;
            manga.Save();

            ajax.ReturnJson(new JsonResponse());
        }
    }
}