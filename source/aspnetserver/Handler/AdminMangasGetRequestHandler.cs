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

            AdminMangasGetRequest request = Utility.ParseJson<AdminMangasGetRequest>(jsonString);
            AdminMangasGetResponse response = new AdminMangasGetResponse();
            if (request.filter == null)
            {
                response.mangas = Manga.ToJsonArray(Manga.GetAllMangas());
            }
            else
            {
                Collection collection = null;
                if (!String.IsNullOrEmpty(request.filter.collection))
                {
                    if ((collection = Collection.GetByName(request.filter.collection)) == null)
                    {
                        ajax.BadRequest();
                        return;
                    }
                }

                response.mangas = Manga.ToJsonArray(Manga.GetMangasWithFilter(collection, request.filter.tag, request.filter.author, request.filter.type));
            }

            ajax.ReturnJson(response);
        }
    }
}