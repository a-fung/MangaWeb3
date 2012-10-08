using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class MangaListItemCoverRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(MangaListItemCoverRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            MangaListItemCoverRequest request = Utility.ParseJson<MangaListItemCoverRequest>(jsonString);
            Manga manga = Manga.GetById(request.id);

            if (manga == null || !manga.ParentCollection.Accessible(ajax))
            {
                ajax.BadRequest();
                return;
            }

            MangaImageResponse response = new MangaImageResponse();

            if (manga.Status == 0)
            {
                string cover = manga.GetCover();
                response.status = cover == null ? 1 : 0;
                response.url = cover;
            }
            else
            {
                response.status = 2;
            }

            ajax.ReturnJson(response);
        }
    }
}