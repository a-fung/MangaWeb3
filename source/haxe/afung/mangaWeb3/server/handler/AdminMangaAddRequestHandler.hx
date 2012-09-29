package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminMangaAddRequest;
import afung.mangaWeb3.common.AdminMangaAddResponse;
import afung.mangaWeb3.server.Collection;
import afung.mangaWeb3.server.Manga;

/**
 * ...
 * @author a-fung
 */

class AdminMangaAddRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminMangaAddRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminMangaAddRequest = Utility.ParseJson(jsonString);
        var collection:Collection = Collection.GetById(request.cid);
        
        if (request.path == null || request.path == "" || collection == null)
        {
            ajax.BadRequest();
            return;
        }
        
        var response:AdminMangaAddResponse = new AdminMangaAddResponse();

        if ((request.path = Manga.CheckMangaPath(request.path)) == null || !Utility.IsValidStringForDatabase(request.path))
        {
            response.status = 1;
        }
        else if (request.path.indexOf(collection.Path) != 0)
        {
            response.status = 2;
        }
        else if (Manga.CheckMangaType(request.path) == -1)
        {
            response.status = 3;
        }
        else
        {
            response.status = 0;
        }
        
        ajax.ReturnJson(response);
    }
}