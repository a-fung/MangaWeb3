using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminCollectionAddRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminCollectionAddRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminCollectionAddRequest request = Utility.ParseJson<AdminCollectionAddRequest>(jsonString);

            if (request.name == null || request.name == "" || request.path == null || request.path == "")
            {
                ajax.BadRequest();
                return;
            }

            AdminCollectionAddResponse response = new AdminCollectionAddResponse();
            request.name = Utility.Remove4PlusBytesUtf8Chars(request.name);

            if (!Collection.CheckNewCollectionName(request.name))
            {
                response.status = 1;
            }
            else if ((request.path = Collection.CheckNewCollectionPath(request.path)) == null || !Utility.IsValidStringForDatabase(request.path))
            {
                response.status = 2;
            }
            else
            {
                response.status = 0;
                Collection.CreateNewCollection(request.name, request.path, request.public_, request.autoadd).Save();
            }

            ajax.ReturnJson(response);
        }
    }
}