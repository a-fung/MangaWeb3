package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminCollectionsUsersGetRequest;
import afung.mangaWeb3.common.AdminCollectionsUsersGetResponse;
import afung.mangaWeb3.server.Collection;
import afung.mangaWeb3.server.CollectionUser;
import afung.mangaWeb3.server.User;
import afung.mangaWeb3.server.Utility;

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
            response.data = CollectionUser.ToJsonArray(CollectionUser.GetByCollection(collection));
            
            var newNames:Array<String> = new Array<String>();
            var exNames:Array<String> = new Array<String>();
            
            for (cu in response.data)
            {
                exNames.push(cu.username);
            }
            
            for (user in User.GetAllUsers())
            {
                if (!Utility.ArrayContains(exNames, user.Username))
                {
                    newNames.push(user.Username);
                }
            }
            
            response.names = newNames;
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
            response.data = CollectionUser.ToJsonArray(CollectionUser.GetByUser(user));
            
            var newNames:Array<String> = new Array<String>();
            var exNames:Array<String> = new Array<String>();
            
            for (cu in response.data)
            {
                exNames.push(cu.collectionName);
            }
            
            for (user in User.GetAllUsers())
            {
                if (!Utility.ArrayContains(exNames, user.Username))
                {
                    newNames.push(user.Username);
                }
            }
            
            response.names = newNames;
        }
        else
        {
            ajax.BadRequest();
            return;
        }

        ajax.ReturnJson(response);
    }
}