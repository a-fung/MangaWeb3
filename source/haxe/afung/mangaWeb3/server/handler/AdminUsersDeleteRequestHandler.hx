package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminUsersDeleteRequest;
import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.server.User;


/**
 * ...
 * @author a-fung
 */

class AdminUsersDeleteRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminUsersDeleteRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminUsersDeleteRequest = Utility.ParseJson(jsonString);

        if (request.ids == null || request.ids.length == 0)
        {
            ajax.BadRequest();
            return;
        }
        
        User.DeleteUsers(request.ids, User.GetCurrentUser(ajax));

        ajax.ReturnJson(new JsonResponse());
    }
}