using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminCollectionsSetPublicRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminCollectionsSetPublicRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminCollectionsSetPublicRequest request = Utility.ParseJson<AdminCollectionsSetPublicRequest>(jsonString);

            if (request.ids == null || request.ids.Length == 0)
            {
                ajax.BadRequest();
                return;
            }

            Collection.SetCollectionsPublic(request.ids, request.public_);

            ajax.ReturnJson(new JsonResponse());
        }
    }
}