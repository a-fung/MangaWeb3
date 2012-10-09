package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.MangaListItemDetailsRequest;
import afung.mangaWeb3.common.MangaListItemDetailsResponse;

/**
 * ...
 * @author a-fung
 */

class MangaListItemDetailsRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return MangaListItemDetailsRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var request:MangaListItemDetailsRequest = Utility.ParseJson(jsonString);
        var manga:Manga = Manga.GetById(request.id);
        
        if (manga == null || !manga.ParentCollection.Accessible(ajax))
        {
            ajax.BadRequest();
            return;
        }

        var response:MangaListItemDetailsResponse = new MangaListItemDetailsResponse();
        response.author = manga.Meta.Author;
        response.series = manga.Meta.Series;
        response.volume = manga.Meta.Volume;
        response.year = manga.Meta.Year;
        response.publisher = manga.Meta.Publisher;
        response.tags = manga.GetTags();
        ajax.ReturnJson(response);
    }
}