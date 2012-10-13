using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class MangaReadRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(MangaReadRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            MangaReadRequest request = Utility.ParseJson<MangaReadRequest>(jsonString);
            MangaReadResponse response = new MangaReadResponse();

            Manga manga = Manga.GetById(request.id);
            Manga nextManga = Manga.GetById(request.nextId);

            if (manga == null || !manga.ParentCollection.Accessible(ajax))
            {
                response.id = -1;
            }
            else
            {
                response.id = manga.Id;
                response.pages = manga.NumberOfPages;
                response.ltr = manga.LeftToRight;
                manga.IncreaseViewCount();
            }

            if (nextManga == null || !nextManga.ParentCollection.Accessible(ajax))
            {
                response.nextId = -1;
            }
            else
            {
                response.nextId = nextManga.Id;
            }

            ajax.ReturnJson(response);
        }
    }
}