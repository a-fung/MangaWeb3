package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.MangaReadRequest;
import afung.mangaWeb3.common.MangaReadResponse;
import afung.mangaWeb3.server.Manga;

/**
 * ...
 * @author a-fung
 */

class MangaReadRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return MangaReadRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var request:MangaReadRequest = Utility.ParseJson(jsonString);
        var response:MangaReadResponse = new MangaReadResponse();

        var manga:Manga = Manga.GetById(request.id);
        var nextManga:Manga = Manga.GetById(request.nextId);

        if (manga == null || !manga.ParentCollection.Accessible(ajax))
        {
            response.id = -1;
        }
        else
        {
            response.id = manga.Id;
            response.pages = manga.NumberOfPages;
            manga.IncreaseViewCount();
        }

        if (nextManga == null || !nextManga.ParentCollection.Accessible(ajax))
        {
            response.nextId = -1;
        }
        else
        {
            response.nextId = nextManga.Id;
        }
        
        ajax.ReturnJson(response);
    }
}