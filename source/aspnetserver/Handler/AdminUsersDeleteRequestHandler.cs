using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminUsersDeleteRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminUsersDeleteRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminUsersDeleteRequest request = Utility.ParseJson<AdminUsersDeleteRequest>(jsonString);

            if (request.ids == null || request.ids.Length == 0)
            {
                ajax.BadRequest();
                return;
            }

            User.DeleteUsers(request.ids, User.GetCurrentUser(ajax));

            ajax.ReturnJson(new JsonResponse());
        }
    }
}