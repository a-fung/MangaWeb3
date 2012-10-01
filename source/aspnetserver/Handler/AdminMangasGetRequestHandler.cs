using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminMangasGetRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminMangasGetRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminMangasGetResponse response = new AdminMangasGetResponse();
            response.mangas = Manga.ToJsonArray(Manga.GetAllMangas());
            ajax.ReturnJson(response);
        }
    }
}