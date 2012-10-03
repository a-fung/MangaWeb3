package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminMangaMetaEditRequest;
import afung.mangaWeb3.common.JsonResponse;

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

        manga.UpdateMeta(request.meta);
        ajax.ReturnJson(new JsonResponse());
    }
}