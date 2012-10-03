using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminMangaFilterRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminMangaFilterRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminMangaFilterResponse response = new AdminMangaFilterResponse();
            response.collections = Collection.GetAllCollectionNames();
            response.tags = Manga.GetAllTags();
            response.authors = MangaMeta.GetAuthors();
            ajax.ReturnJson(response);
        }
    }
}