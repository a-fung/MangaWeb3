package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminUsersSetAdminRequest;
import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.server.User;

/**
 * ...
 * @author a-fung
 */

class AdminUsersSetAdminRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminUsersSetAdminRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminUsersSetAdminRequest = Utility.ParseJson(jsonString);

        if (request.ids == null || request.ids.length == 0)
        {
            ajax.BadRequest();
            return;
        }

        User.SetAdmin(request.ids, request.admin, User.GetCurrentUser(ajax));

        ajax.ReturnJson(new JsonResponse());
    }
}