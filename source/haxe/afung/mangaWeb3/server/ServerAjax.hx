package afung.mangaWeb3.server;

import afung.mangaWeb3.server.handler.AdminCollectionAddRequestHandler;
import afung.mangaWeb3.server.handler.AdminCollectionEditNameRequestHandler;
import afung.mangaWeb3.server.handler.AdminCollectionsDeleteRequestHandler;
import afung.mangaWeb3.server.handler.AdminCollectionsGetRequestHandler;
import afung.mangaWeb3.server.handler.AdminCollectionsSetPublicRequestHandler;
import afung.mangaWeb3.server.handler.AdminCollectionsUsersAccessRequestHandler;
import afung.mangaWeb3.server.handler.AdminCollectionsUsersDeleteRequestHandler;
import afung.mangaWeb3.server.handler.AdminCollectionsUsersGetRequestHandler;
import afung.mangaWeb3.server.handler.AdminCollectionUserAddRequestHandler;
import afung.mangaWeb3.server.handler.AdminFinderRequestHandler;
import afung.mangaWeb3.server.handler.AdminMangaAddRequestHandler;
import afung.mangaWeb3.server.handler.AdminMangaEditPathRequestHandler;
import afung.mangaWeb3.server.handler.AdminMangaFilterRequestHandler;
import afung.mangaWeb3.server.handler.AdminMangaMetaEditRequestHandler;
import afung.mangaWeb3.server.handler.AdminMangaMetaGetRequestHandler;
import afung.mangaWeb3.server.handler.AdminMangasDeleteRequestHandler;
import afung.mangaWeb3.server.handler.AdminMangasGetRequestHandler;
import afung.mangaWeb3.server.handler.AdminMangasRefreshRequestHandler;
import afung.mangaWeb3.server.handler.AdminSettingsGetRequestHandler;
import afung.mangaWeb3.server.handler.AdminSettingsSetRequestHandler;
import afung.mangaWeb3.server.handler.AdminUserAddRequestHandler;
import afung.mangaWeb3.server.handler.AdminUsersDeleteRequestHandler;
import afung.mangaWeb3.server.handler.AdminUsersGetRequestHandler;
import afung.mangaWeb3.server.handler.AdminUsersSetAdminRequestHandler;
import afung.mangaWeb3.server.handler.ChangePasswordRequestHandler;
import afung.mangaWeb3.server.handler.HandlerBase;
import afung.mangaWeb3.server.handler.LoginRequestHandler;
import afung.mangaWeb3.server.install.InstallAjax;

/**
 * ...
 * @author a-fung
 */

class ServerAjax extends AjaxBase
{
    public function new()
    {
        super();
    }
    
    public static function main() 
    {
        new ServerAjax().Page_Load();
    }
    
    private static var handlers:Array<HandlerBase> = null;
    
    public override function PageLoad():Void 
    {
        if (!Config.IsInstalled)
        {
            BadRequest();
            return;
        }

        if (handlers == null)
        {
            handlers = [
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
            ];
        }

        HandleRequest(handlers);
    }
}