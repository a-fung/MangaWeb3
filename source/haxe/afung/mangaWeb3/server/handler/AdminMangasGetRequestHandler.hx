package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminMangasGetRequest;
import afung.mangaWeb3.common.AdminMangasGetResponse;
import afung.mangaWeb3.server.Collection;

/**
 * ...
 * @author a-fung
 */

class AdminMangasGetRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminMangasGetRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminMangasGetRequest = Utility.ParseJson(jsonString);
        var response:AdminMangasGetResponse = new AdminMangasGetResponse();
        if (request.filter == null)
        {
            response.mangas = Manga.ToJsonArray(Manga.GetAllMangas());
        }
        else
        {
            var collection:Collection = null;
            if (request.filter.collection != null && request.filter.collection != "")
            {
                if ((collection = Collection.GetByName(request.filter.collection)) == null)
                {
                    ajax.BadRequest();
                    return;
                }
            }

            response.mangas = Manga.ToJsonArray(Manga.GetMangasWithFilter(collection, request.filter.tag, request.filter.author, request.filter.type));
        }
            
        ajax.ReturnJson(response);
    }
}