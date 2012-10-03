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

            manga.UpdateMeta(request.meta);
            ajax.ReturnJson(new JsonResponse());
        }
    }
}