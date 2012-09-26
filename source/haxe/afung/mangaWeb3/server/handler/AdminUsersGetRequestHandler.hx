package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminUsersGetRequest;
import afung.mangaWeb3.common.AdminUsersGetResponse;
import afung.mangaWeb3.server.User;

/**
 * ...
 * @author a-fung
 */

class AdminUsersGetRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminUsersGetRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var response:AdminUsersGetResponse = new AdminUsersGetResponse();
        response.users = User.ToJsonArray(User.GetAllUsers());
        ajax.ReturnJson(response);
    }
}