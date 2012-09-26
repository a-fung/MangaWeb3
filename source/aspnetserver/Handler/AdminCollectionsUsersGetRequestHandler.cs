using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminCollectionsUsersGetRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminCollectionsUsersGetRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminCollectionsUsersGetRequest request = Utility.ParseJson<AdminCollectionsUsersGetRequest>(jsonString);
            AdminCollectionsUsersGetResponse response = new AdminCollectionsUsersGetResponse();

            if (request.t == 0)
            {
                Collection collection = Collection.GetById(request.id);
                if (collection == null)
                {
                    ajax.BadRequest();
                    return;
                }

                response.name = collection.Name;
                response.data = CollectionUser.ToJsonArray(CollectionUser.GetByCollection(collection));

                List<string> newNames = new List<string>();
                List<string> exNames = new List<string>();
                foreach (CollectionUserJson cu in response.data)
                {
                    exNames.Add(cu.username);
                }

                foreach (User user in User.GetAllUsers())
                {
                    if (!exNames.Contains(user.Username))
                    {
                        newNames.Add(user.Username);
                    }
                }

                response.names = newNames.ToArray();
            }
            else if (request.t == 1)
            {
                User user = User.GetById(request.id);
                if (user == null)
                {
                    ajax.BadRequest();
                    return;
                }

                response.name = user.Username;
                response.data = CollectionUser.ToJsonArray(CollectionUser.GetByUser(user));

                List<string> newNames = new List<string>();
                List<string> exNames = new List<string>();
                foreach (CollectionUserJson cu in response.data)
                {
                    exNames.Add(cu.collectionName);
                }

                foreach (Collection collection in Collection.GetAllCollections())
                {
                    if (!exNames.Contains(collection.Name))
                    {
                        newNames.Add(collection.Name);
                    }
                }

                response.names = newNames.ToArray();
            }
            else
            {
                ajax.BadRequest();
                return;
            }

            ajax.ReturnJson(response);
        }
    }
}