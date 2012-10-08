using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using afung.MangaWeb3.Server.Handler;

namespace afung.MangaWeb3.Server
{
    public partial class ServerAjax : AjaxBase
    {
        private static HandlerBase[] handlers = null;

        protected override void PageLoad()
        {
            if (!Config.IsInstalled)
            {
                BadRequest();
                return;
            }

            if (handlers == null)
            {
                handlers = new HandlerBase[]{
                    new MangaListItemCoverRequestHandler(),
                    new MangaListRequestHandler(),
                    new LoginRequestHandler(),
                    new ChangePasswordRequestHandler(),
                    new AdminSettingsGetRequestHandler(),
                    new AdminSettingsSetRequestHandler(),
                    new AdminCollectionAddRequestHandler(),
                    new AdminCollectionsGetRequestHandler(),
                    new AdminCollectionsDeleteRequestHandler(),
                    new AdminCollectionsSetPublicRequestHandler(),
                    new AdminCollectionEditNameRequestHandler(),
                    new AdminUsersGetRequestHandler(),
                    new AdminUserAddRequestHandler(),
                    new AdminUsersDeleteRequestHandler(),
                    new AdminUsersSetAdminRequestHandler(),
                    new AdminCollectionsUsersGetRequestHandler(),
                    new AdminCollectionUserAddRequestHandler(),
                    new AdminCollectionsUsersDeleteRequestHandler(),
                    new AdminCollectionsUsersAccessRequestHandler(),
                    new AdminMangaAddRequestHandler(),
                    new AdminFinderRequestHandler(),
                    new AdminMangasGetRequestHandler(),
                    new AdminMangasDeleteRequestHandler(),
                    new AdminMangasRefreshRequestHandler(),
                    new AdminMangaEditPathRequestHandler(),
                    new AdminMangaMetaGetRequestHandler(),
                    new AdminMangaMetaEditRequestHandler(),
                    new AdminMangaFilterRequestHandler(),
                };
            }

            HandleRequest(handlers);
        }
    }
}