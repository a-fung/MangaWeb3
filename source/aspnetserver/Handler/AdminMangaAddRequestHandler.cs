using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminMangaAddRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminMangaAddRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminMangaAddRequest request = Utility.ParseJson<AdminMangaAddRequest>(jsonString);
            Collection collection = Collection.GetById(request.cid);

            if (request.path == null || request.path == "" || collection == null)
            {
                ajax.BadRequest();
                return;
            }

            AdminMangaAddResponse response = new AdminMangaAddResponse();

            if ((request.path = Manga.CheckMangaPath(request.path)) == null || !Utility.IsValidStringForDatabase(request.path))
            {
                response.status = 1;
            }
            else if (request.path.IndexOf(collection.Path, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                response.status = 2;
            }
            else if (Manga.CheckMangaType(request.path) == -1)
            {
                response.status = 3;
            }
            else
            {
                response.status = 0;
            }

            ajax.ReturnJson(response);
        }
    }
}