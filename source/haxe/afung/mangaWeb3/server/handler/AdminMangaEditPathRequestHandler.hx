package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminMangaEditPathRequest;
import afung.mangaWeb3.common.AdminMangaEditPathResponse;
import afung.mangaWeb3.server.Manga;

/**
 * ...
 * @author a-fung
 */

class AdminMangaEditPathRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminMangaEditPathRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminMangaEditPathRequest = Utility.ParseJson(jsonString);
        var manga:Manga = Manga.GetById(request.id);

        if (manga == null)
        {
            ajax.BadRequest();
            return;
        }

        var response:AdminMangaEditPathResponse = new AdminMangaEditPathResponse();

        if (request.path == null || request.path == "")
        {
            response.path = manga.MangaPath;
            response.cid = manga.ParentCollectionId;
        }
        else
        {
            var mangaType:Int;

            if ((request.path = Manga.CheckMangaPath(request.path)) == null || !Utility.IsValidStringForDatabase(request.path))
            {
                response.status = 1;
            }
            else if (request.path.indexOf(manga.ParentCollection.Path) != 0)
            {
                ajax.BadRequest();
                return;
            }
            else if ((mangaType = Manga.CheckMangaType(request.path)) == -1)
            {
                response.status = 3;
            }
            else
            {
                response.status = 0;
                manga.ChangePath(request.path, mangaType);
            }
        }

        ajax.ReturnJson(response);
    }
}