using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminMangaMetaEditRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminMangaMetaEditRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminMangaMetaEditRequest request = Utility.ParseJson<AdminMangaMetaEditRequest>(jsonString);
            Manga manga = Manga.GetById(request.id);
            if (manga == null || request.meta == null)
            {
                ajax.BadRequest();
                return;
            }

            if (request.meta.author.Length > 100)
            {
                request.meta.author = request.meta.author.Substring(0, 100);
            }

            if (request.meta.title.Length > 100)
            {
                request.meta.title = request.meta.title.Substring(0, 100);
            }

            if (request.meta.series.Length > 100)
            {
                request.meta.series = request.meta.series.Substring(0, 100);
            }

            if (request.meta.publisher.Length > 100)
            {
                request.meta.publisher = request.meta.publisher.Substring(0, 100);
            }

            if (request.meta.volume < -1)
            {
                request.meta.volume = -1;
            }

            if (request.meta.volume > 999999999)
            {
                request.meta.volume = 999999999;
            }

            if (request.meta.year < -1)
            {
                request.meta.year = -1;
            }

            if (request.meta.year > 9999)
            {
                request.meta.year = 9999;
            }

            manga.UpdateMeta(request.meta);
            ajax.ReturnJson(new JsonResponse());
        }
    }
}