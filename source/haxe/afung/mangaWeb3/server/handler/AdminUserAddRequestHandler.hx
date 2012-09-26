package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminUserAddRequest;
import afung.mangaWeb3.common.AdminUserAddResponse;
import afung.mangaWeb3.server.User;

/**
 * ...
 * @author a-fung
 */

class AdminUserAddRequestHandler extends HandlerBase
{    
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminUserAddRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminUserAddRequest = Utility.ParseJson(jsonString);
        var regex:EReg = ~/[^a-zA-Z0-9]/;
        if (request.username == null || request.username == "" || regex.match(request.username) || request.password == null || request.password.length < 8 || request.password != request.password2)
        {
            ajax.BadRequest();
            return;
        }
        
        var response:AdminUserAddResponse = new AdminUserAddResponse();
        
        if (User.GetUser(request.username) != null)
        {
            response.status = 1;
        }
        else
        {
            response.status = 0;
            User.CreateNewUser(request.username, request.password, request.admin).Save();
        }

        ajax.ReturnJson(response);
        
    }
}