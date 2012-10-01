using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminMangaEditPathRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminMangaEditPathRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminMangaEditPathRequest request = Utility.ParseJson<AdminMangaEditPathRequest>(jsonString);
            Manga manga = Manga.GetById(request.id);

            if (manga == null)
            {
                ajax.BadRequest();
                return;
            }

            AdminMangaEditPathResponse response = new AdminMangaEditPathResponse();

            if (request.path == null || request.path == "")
            {
                response.path = manga.MangaPath;
                response.cid = manga.ParentCollection.Id;
            }
            else
            {
                int mangaType;

                if ((request.path = Manga.CheckMangaPath(request.path)) == null || !Utility.IsValidStringForDatabase(request.path))
                {
                    response.status = 1;
                }
                else if (request.path.IndexOf(manga.ParentCollection.Path, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    ajax.BadRequest();
                    return;
                }
                else if ((mangaType = Manga.CheckMangaType(request.path)) == -1)
                {
                    response.status = 3;
                }
                else
                {
                    response.status = 0;
                    manga.ChangePath(request.path, mangaType);
                }
            }

            ajax.ReturnJson(response);
        }
    }
}