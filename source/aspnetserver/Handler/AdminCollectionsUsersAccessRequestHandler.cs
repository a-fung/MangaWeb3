using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminCollectionsUsersAccessRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminCollectionsUsersAccessRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminCollectionsUsersAccessRequest request = Utility.ParseJson<AdminCollectionsUsersAccessRequest>(jsonString);

            if (request.ids == null || request.ids.Length == 0 || (request.t != 0 && request.t != 1))
            {
                ajax.BadRequest();
                return;
            }

            CollectionUser.SetRelationsAcess(request.t, request.id, request.ids, request.access);

            ajax.ReturnJson(new JsonResponse());
        }
    }
}