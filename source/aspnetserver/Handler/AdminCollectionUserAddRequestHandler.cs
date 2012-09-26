using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminCollectionUserAddRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminCollectionUserAddRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminCollectionUserAddRequest request = Utility.ParseJson<AdminCollectionUserAddRequest>(jsonString);
            Collection collection = Collection.GetByName(request.collectionName);
            User user = User.GetUser(request.username);

            if (collection == null || user == null || CollectionUser.Get(collection, user) != null)
            {
                ajax.BadRequest();
                return;
            }

            CollectionUser.CreateNew(collection, user, request.access).Save();
            ajax.ReturnJson(new JsonResponse());
        }
    }
}