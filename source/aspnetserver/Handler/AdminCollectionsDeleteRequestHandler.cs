using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminCollectionsDeleteRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminCollectionsDeleteRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminCollectionsDeleteRequest request = Utility.ParseJson<AdminCollectionsDeleteRequest>(jsonString);

            if (request.ids == null || request.ids.Length == 0)
            {
                ajax.BadRequest();
                return;
            }

            Collection.DeleteCollections(request.ids);

            ajax.ReturnJson(new JsonResponse());
        }
    }
}