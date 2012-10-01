using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminMangasRefreshRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminMangasRefreshRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminMangasRefreshRequest request = Utility.ParseJson<AdminMangasRefreshRequest>(jsonString);

            if (request.ids == null || request.ids.Length == 0)
            {
                ajax.BadRequest();
                return;
            }

            Manga.RefreshMangasContent(request.ids);

            ajax.ReturnJson(new JsonResponse());
        }
    }
}