package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.MangaImageResponse;
import afung.mangaWeb3.common.MangaPageRequest;
import afung.mangaWeb3.server.Manga;

/**
 * ...
 * @author a-fung
 */

class MangaPageRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return MangaPageRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var request:MangaPageRequest = Utility.ParseJson(jsonString);
        var manga:Manga = Manga.GetById(request.id);

        if (manga == null || request.page < 0 || request.page >= manga.NumberOfPages || (request.width <= 0 && request.height <= 0))
        {
            ajax.BadRequest();
            return;
        }

        if (!manga.ParentCollection.Accessible(ajax))
        {
            ajax.Unauthorized();
            return;
        }

        var response:MangaImageResponse = new MangaImageResponse();

        if (manga.Status == 0)
        {
            var page:String = manga.GetPage(request.page, request.width, request.height);
            response.status = page == null ? 1 : 0;
            response.url = page;
        }
        else
        {
            response.status = 2;
        }
        
        ajax.ReturnJson(response);
    }
}