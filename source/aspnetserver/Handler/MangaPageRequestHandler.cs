using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class MangaPageRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(MangaPageRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            MangaPageRequest request = Utility.ParseJson<MangaPageRequest>(jsonString);
            Manga manga = Manga.GetById(request.id);

            if (manga == null || request.page < 0 || request.page >= manga.NumberOfPages || (request.width <= 0 && request.height <= 0))
            {
                ajax.BadRequest();
                return;
            }

            if (!manga.ParentCollection.Accessible(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            MangaImageResponse response = new MangaImageResponse();

            if (manga.Status == 0)
            {
                string page = manga.GetPage(request.page, request.width, request.height, request.part);
                response.status = page == null ? 1 : 0;
                response.url = page;
                response.dimensions = request.dimensions && request.part == 0 && page != null ? manga.GetDimensions(request.page) : null;
                
                ThreadHelper.Run(request.part == 0 ? "MangaPreprocessFiles" : "MangaPreprocessParts", request.id, request.page, request.width, request.height);
            }
            else
            {
                response.status = 2;
            }

            ajax.ReturnJson(response);
        }
    }
}