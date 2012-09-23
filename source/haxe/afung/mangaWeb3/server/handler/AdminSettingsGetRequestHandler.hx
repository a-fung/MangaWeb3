package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminSettingsGetRequest;
import afung.mangaWeb3.common.AdminSettingsGetResponse;

/**
 * ...
 * @author a-fung
 */

class AdminSettingsGetRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminSettingsGetRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var response:AdminSettingsGetResponse = new AdminSettingsGetResponse();
        response.guest = Settings.AllowGuest;
        response.zip = Settings.UseZip;
        response.rar = Settings.UseRar;
        response.pdf = Settings.UsePdf;

        ajax.ReturnJson(response);
    }
}