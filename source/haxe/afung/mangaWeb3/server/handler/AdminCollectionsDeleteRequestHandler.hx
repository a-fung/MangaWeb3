package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminCollectionsDeleteRequest;
import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.server.Collection;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionsDeleteRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminCollectionsDeleteRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminCollectionsDeleteRequest = Utility.ParseJson(jsonString);

        if (request.ids == null || request.ids.length == 0)
        {
            ajax.BadRequest();
            return;
        }
        
        Collection.DeleteCollections(request.ids);

        ajax.ReturnJson(new JsonResponse());
    }
}