package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.MangaImageResponse;
import afung.mangaWeb3.common.MangaListItemCoverRequest;
import afung.mangaWeb3.server.Manga;

/**
 * ...
 * @author a-fung
 */

class MangaListItemCoverRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return MangaListItemCoverRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var request:MangaListItemCoverRequest = Utility.ParseJson(jsonString);
        var manga:Manga = Manga.GetById(request.id);
        
        if (manga == null || !manga.ParentCollection.Accessible(ajax))
        {
            ajax.BadRequest();
            return;
        }

        var response:MangaImageResponse = new MangaImageResponse();

        if (manga.Status == 0)
        {
            var cover:String = manga.GetCover();
            response.status = cover == null ? 1 : 0;
            response.url = cover;
        }
        else
        {
            response.status = 2;
        }
        
        ajax.ReturnJson(response);
    }
    
}