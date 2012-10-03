package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminMangaFilterRequest;
import afung.mangaWeb3.common.AdminMangaFilterResponse;
import afung.mangaWeb3.server.Collection;
import afung.mangaWeb3.server.Manga;
import afung.mangaWeb3.server.MangaMeta;

/**
 * ...
 * @author a-fung
 */

class AdminMangaFilterRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminMangaFilterRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var response:AdminMangaFilterResponse = new AdminMangaFilterResponse();
        response.collections = Collection.GetAllCollectionNames();
        response.tags = Manga.GetAllTags();
        response.authors = MangaMeta.GetAuthors();
        ajax.ReturnJson(response);
    }
}