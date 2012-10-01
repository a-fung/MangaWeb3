package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminMangasDeleteRequest;
import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.server.Manga;

/**
 * ...
 * @author a-fung
 */

class AdminMangasDeleteRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminMangasDeleteRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminMangasDeleteRequest = Utility.ParseJson(jsonString);

        if (request.ids == null || request.ids.length == 0)
        {
            ajax.BadRequest();
            return;
        }
        
        Manga.DeleteMangasFromIds(request.ids);

        ajax.ReturnJson(new JsonResponse());
    }
}