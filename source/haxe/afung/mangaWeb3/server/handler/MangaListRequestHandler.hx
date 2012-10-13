package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.MangaListRequest;
import afung.mangaWeb3.common.MangaListResponse;
import afung.mangaWeb3.server.Manga;

/**
 * ...
 * @author a-fung
 */

class MangaListRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return MangaListRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var request:MangaListRequest = Utility.ParseJson(jsonString);
        var response:MangaListResponse = new MangaListResponse();
        response.items = Manga.ToListItemJsonArray(Manga.GetMangaList(ajax, request.filter));
        
        if (request.filter == null ||
            (
                (request.filter.folder == null || request.filter.folder == "") &&
                (request.filter.tag == null || request.filter.tag == "") &&
                request.filter.search == null
            ))
        {
            ThreadHelper.Run("ProcessAutoAddStage1", []);
        }
        
        ajax.ReturnJson(response);
    }
}