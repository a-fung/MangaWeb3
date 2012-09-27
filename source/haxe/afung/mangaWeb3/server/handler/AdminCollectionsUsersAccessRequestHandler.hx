package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminCollectionsUsersAccessRequest;
import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.server.CollectionUser;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionsUsersAccessRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminCollectionsUsersAccessRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminCollectionsUsersAccessRequest = Utility.ParseJson(jsonString);

        if (request.ids == null || request.ids.length == 0 || (request.t != 0 && request.t != 1))
        {
            ajax.BadRequest();
            return;
        }

        CollectionUser.SetRelationsAccess(request.t, request.id, request.ids, request.access);

        ajax.ReturnJson(new JsonResponse());
    }
}