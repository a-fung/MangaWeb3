package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminCollectionUserAddRequest;
import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.server.Collection;
import afung.mangaWeb3.server.CollectionUser;
import afung.mangaWeb3.server.User;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionUserAddRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminCollectionUserAddRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminCollectionUserAddRequest = Utility.ParseJson(jsonString);
        var collection:Collection = Collection.GetByName(request.collectionName);
        var user:User = User.GetUser(request.username);

        if (collection == null || user == null || CollectionUser.Get(collection, user) != null)
        {
            ajax.BadRequest();
            return;
        }
        
        CollectionUser.CreateNew(collection, user, request.access).Save();
        ajax.ReturnJson(new JsonResponse());
    }
}