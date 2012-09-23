package afung.mangaWeb3.server;

import afung.mangaWeb3.common.ErrorResponse;
import afung.mangaWeb3.common.PreInstallCheckRequest;
import afung.mangaWeb3.server.handler.AdminCollectionAddRequestHandler;
import afung.mangaWeb3.server.handler.AdminCollectionsGetRequestHandler;
import afung.mangaWeb3.server.handler.AdminSettingsGetRequestHandler;
import afung.mangaWeb3.server.handler.AdminSettingsSetRequestHandler;
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
                new AdminSettingsGetRequestHandler(),
                new AdminSettingsSetRequestHandler(),
                new AdminCollectionAddRequestHandler(),
                new AdminCollectionsGetRequestHandler()
            ];
        }

        HandleRequest(handlers);
    }
}