package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminCollectionsGetRequest;
import afung.mangaWeb3.common.AdminCollectionsGetResponse;
import afung.mangaWeb3.server.Collection;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionsGetRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminCollectionsGetRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var response:AdminCollectionsGetResponse = new AdminCollectionsGetResponse();
        response.collections = Collection.ToJsonArray(Collection.GetAllCollections());
        ajax.ReturnJson(response);
    }
}