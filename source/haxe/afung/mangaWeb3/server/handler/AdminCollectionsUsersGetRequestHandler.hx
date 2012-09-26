package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminCollectionsUsersGetRequest;
import afung.mangaWeb3.common.AdminCollectionsUsersGetResponse;
import afung.mangaWeb3.server.Collection;
import afung.mangaWeb3.server.CollectionUser;
import afung.mangaWeb3.server.User;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionsUsersGetRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminCollectionsUsersGetRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminCollectionsUsersGetRequest = Utility.ParseJson(jsonString);
        var response:AdminCollectionsUsersGetResponse = new AdminCollectionsUsersGetResponse();

        if (request.t == 0)
        {
            var collection:Collection = Collection.GetById(request.id);
            if (collection == null)
            {
                ajax.BadRequest();
                return;
            }
            
            response.name = collection.Name;
        }
        else if (request.t == 1)
        {
            var user:User = User.GetById(request.id);
            if (user == null)
            {
                ajax.BadRequest();
                return;
            }
            
            response.name = user.Username;
        }
        else
        {
            ajax.BadRequest();
            return;
        }

        ajax.ReturnJson(response);
    }
}