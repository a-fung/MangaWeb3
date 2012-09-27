using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminCollectionsUsersDeleteRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminCollectionsUsersDeleteRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminCollectionsUsersDeleteRequest request = Utility.ParseJson<AdminCollectionsUsersDeleteRequest>(jsonString);

            if (request.ids == null || request.ids.Length == 0 || (request.t != 0 && request.t != 1))
            {
                ajax.BadRequest();
                return;
            }

            CollectionUser.DeleteRelations(request.t, request.id, request.ids);

            ajax.ReturnJson(new JsonResponse());
        }
    }
}