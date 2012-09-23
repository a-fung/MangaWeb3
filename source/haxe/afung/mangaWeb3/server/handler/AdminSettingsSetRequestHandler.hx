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

        ajax.ReturnJson(new JsonResponse());
    }
}