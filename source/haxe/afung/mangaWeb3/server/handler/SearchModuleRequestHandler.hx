package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.SearchModuleRequest;
import afung.mangaWeb3.common.SearchModuleResponse;
import afung.mangaWeb3.server.User;

/**
 * ...
 * @author a-fung
 */

class SearchModuleRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return SearchModuleRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var response:SearchModuleResponse = new SearchModuleResponse();

        var mangaWhere:String = "`status`='0'";
        var user:User = User.GetCurrentUser(ajax);
        var collectionSelect:String = "FALSE";
        if (Settings.AllowGuest || user != null)
        {
            collectionSelect += " OR `cid` IN (SELECT `id` FROM `collection` WHERE `public`='1')";
        }

        if (user != null)
        {
            collectionSelect += " OR `cid` IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(Std.string(user.Id)) + " AND `access`='1')";
            mangaWhere += " AND `cid` NOT IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(Std.string(user.Id)) + " AND `access`='0')";
        }

        mangaWhere += " AND (" + collectionSelect + ")";

        var where:String = "`mid` IN (SELECT `id` FROM `manga` WHERE " + mangaWhere + ")";

        response.authors = Database.GetDistinctStringValues("meta", "author", where);
        response.series = Database.GetDistinctStringValues("meta", "series", where);
        response.publishers = Database.GetDistinctStringValues("meta", "publisher", where);
        
        ajax.ReturnJson(response);
    }
}