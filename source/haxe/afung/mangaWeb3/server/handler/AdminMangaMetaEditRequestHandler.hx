package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminMangaMetaEditRequest;
import afung.mangaWeb3.common.JsonResponse;
import haxe.Utf8;

/**
 * ...
 * @author a-fung
 */

class AdminMangaMetaEditRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminMangaMetaEditRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminMangaMetaEditRequest = Utility.ParseJson(jsonString);
        var manga:Manga = Manga.GetById(request.id);
        if (manga == null || request.meta == null)
        {
            ajax.BadRequest();
            return;
        }
        
        if (Utf8.length(request.meta.author) > 100)
        {
            request.meta.author = Utf8.sub(request.meta.author, 0, 100);
        }
        
        if (Utf8.length(request.meta.title) > 100)
        {
            request.meta.title = Utf8.sub(request.meta.title, 0, 100);
        }
        
        if (Utf8.length(request.meta.series) > 100)
        {
            request.meta.series = Utf8.sub(request.meta.series, 0, 100);
        }
        
        if (Utf8.length(request.meta.publisher) > 100)
        {
            request.meta.publisher = Utf8.sub(request.meta.publisher, 0, 100);
        }
        
        if (request.meta.volume < -1)
        {
            request.meta.volume = -1;
        }
        
        if (request.meta.volume > 999999999)
        {
            request.meta.volume = 999999999;
        }
        
        if (request.meta.year < -1)
        {
            request.meta.year = -1;
        }
        
        if (request.meta.year > 9999)
        {
            request.meta.year = 9999;
        }
        
        manga.UpdateMeta(request.meta);
        ajax.ReturnJson(new JsonResponse());
    }
}