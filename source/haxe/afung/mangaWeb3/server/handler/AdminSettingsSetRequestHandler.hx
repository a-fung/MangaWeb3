package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminSettingsSetRequest;
import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.server.Settings;

/**
 * ...
 * @author a-fung
 */

class AdminSettingsSetRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminSettingsSetRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminSettingsSetRequest = Utility.ParseJson(jsonString);

        Settings.AllowGuest = request.guest;
        Settings.UseZip = request.zip;
        Settings.UseRar = request.rar;
        Settings.UsePdf = request.pdf;
        
        if (request.preprocessCount >= 0 && request.preprocessCount < 100)
        {
            Settings.MangaPagePreProcessCount = request.preprocessCount;
        }
        
        if (request.preprocessDelay >= 0 && request.preprocessDelay < 100000)
        {
            Settings.MangaPagePreProcessDelay = request.preprocessDelay;
        }
        
        if (request.cacheLimit >= 50 && request.cacheLimit < 100000)
        {
            Settings.MangaCacheSizeLimit = request.cacheLimit;
        }

        ajax.ReturnJson(new JsonResponse());
    }
}