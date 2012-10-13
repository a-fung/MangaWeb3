package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.common.MangaDirectionRequest;
import afung.mangaWeb3.server.Manga;

/**
 * ...
 * @author a-fung
 */

class MangaDirectionRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return MangaDirectionRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var request:MangaDirectionRequest = Utility.ParseJson(jsonString);

        var manga:Manga = Manga.GetById(request.id);

        if (manga == null || !manga.ParentCollection.Accessible(ajax))
        {
            ajax.BadRequest();
            return;
        }

        manga.LeftToRight = !manga.LeftToRight;
        manga.Save();
        
        ajax.ReturnJson(new JsonResponse());
    }
}