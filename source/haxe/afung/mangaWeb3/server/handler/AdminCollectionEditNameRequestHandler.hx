package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminCollectionEditNameRequest;
import afung.mangaWeb3.common.AdminCollectionEditNameResponse;
import afung.mangaWeb3.server.Collection;
import haxe.Utf8;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionEditNameRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminCollectionEditNameRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminCollectionEditNameRequest = Utility.ParseJson(jsonString);
        var collection:Collection = Collection.GetById(request.id);

        if (collection == null)
        {
            ajax.BadRequest();
            return;
        }
        
        var response:AdminCollectionEditNameResponse = new AdminCollectionEditNameResponse();
        
        if (request.name == null || request.name == "")
        {
            response.name = collection.Name;
        }
        else
        {
            request.name = Utility.Remove4PlusBytesUtf8Chars(request.name);
            
            if (Utf8.length(request.name) > 50)
            {
                request.name = Utf8.sub(request.name, 0, 50);
            }
            
            if (!Collection.CheckNewCollectionName(request.name))
            {
                response.status = 1;
            }
            else
            {
                response.status = 0;
                collection.Name = request.name;
                collection.Save();
            }
        }
        
        ajax.ReturnJson(response);
    }
}