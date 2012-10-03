using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminMangaMetaGetRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminMangaMetaGetRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminMangaMetaGetRequest request = Utility.ParseJson<AdminMangaMetaGetRequest>(jsonString);
            Manga manga = Manga.GetById(request.id);
            if (manga == null)
            {
                ajax.BadRequest();
                return;
            }

            AdminMangaMetaGetResponse response = new AdminMangaMetaGetResponse();
            response.meta = manga.GetMetaJson();
            response.authors = MangaMeta.GetAuthors();
            response.series = MangaMeta.GetSeries();
            response.publishers = MangaMeta.GetPublishers();
            ajax.ReturnJson(response);
        }
    }
}