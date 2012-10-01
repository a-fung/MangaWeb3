package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminMangasGetRequest;
import afung.mangaWeb3.common.AdminMangasGetResponse;

/**
 * ...
 * @author a-fung
 */

class AdminMangasGetRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminMangasGetRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var response:AdminMangasGetResponse = new AdminMangasGetResponse();
        response.mangas = Manga.ToJsonArray(Manga.GetAllMangas());
        ajax.ReturnJson(response);
    }
}