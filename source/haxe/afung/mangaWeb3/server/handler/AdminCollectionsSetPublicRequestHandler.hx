package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminCollectionsSetPublicRequest;
import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.server.Collection;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionsSetPublicRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminCollectionsSetPublicRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminCollectionsSetPublicRequest = Utility.ParseJson(jsonString);

        if (request.ids == null || request.ids.length == 0)
        {
            ajax.BadRequest();
            return;
        }

        Collection.SetCollectionsPublic(request.ids, request.public_);

        ajax.ReturnJson(new JsonResponse());
    }
}