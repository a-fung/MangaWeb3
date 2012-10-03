package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminMangaMetaGetRequest;
import afung.mangaWeb3.common.AdminMangaMetaGetResponse;
import afung.mangaWeb3.server.Manga;
import afung.mangaWeb3.server.MangaMeta;

/**
 * ...
 * @author a-fung
 */

class AdminMangaMetaGetRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminMangaMetaGetRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminMangaMetaGetRequest = Utility.ParseJson(jsonString);
        var manga:Manga = Manga.GetById(request.id);
        if (manga == null)
        {
            ajax.BadRequest();
            return;
        }

        var response:AdminMangaMetaGetResponse = new AdminMangaMetaGetResponse();
        response.meta = manga.GetMetaJson();
        response.authors = MangaMeta.GetAuthors();
        response.series = MangaMeta.GetSeries();
        response.publishers = MangaMeta.GetPublishers();
        ajax.ReturnJson(response);
    }
}