using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminMangasDeleteRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminMangasDeleteRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminMangasDeleteRequest request = Utility.ParseJson<AdminMangasDeleteRequest>(jsonString);

            if (request.ids == null || request.ids.Length == 0)
            {
                ajax.BadRequest();
                return;
            }

            Manga.DeleteMangasFromIds(request.ids);

            ajax.ReturnJson(new JsonResponse());
        }
    }
}