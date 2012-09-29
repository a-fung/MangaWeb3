package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminCollectionAddRequest;
import afung.mangaWeb3.common.AdminCollectionAddResponse;
import afung.mangaWeb3.server.Collection;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionAddRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminCollectionAddRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminCollectionAddRequest = Utility.ParseJson(jsonString);
        
        if (request.name == null || request.name == "" || request.path == null || request.path == "")
        {
            ajax.BadRequest();
            return;
        }
        
        var response:AdminCollectionAddResponse = new AdminCollectionAddResponse();
        request.name = Utility.Remove4PlusBytesUtf8Chars(request.name);

        if (!Collection.CheckNewCollectionName(request.name))
        {
            response.status = 1;
        }
        else if ((request.path = Collection.CheckNewCollectionPath(request.path)) == null || !Utility.IsValidStringForDatabase(request.path))
        {
            response.status = 2;
        }
        else
        {
            response.status = 0;
            Collection.CreateNewCollection(request.name, request.path, request.public_, request.autoadd).Save();
        }

        ajax.ReturnJson(response);
    }
}