package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.FolderJson;
import afung.mangaWeb3.common.FolderRequest;
import afung.mangaWeb3.common.FolderResponse;
import afung.mangaWeb3.server.Collection;
import afung.mangaWeb3.server.Utility;

/**
 * ...
 * @author a-fung
 */

class FolderRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return FolderRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var response:FolderResponse = new FolderResponse();
        var collections:Array<Collection> = Collection.GetAccessible(ajax);
        var folders:Array<FolderJson> = new Array<FolderJson>();
        
        var ready:Bool = true;
        for (collection in collections)
        {
            var folderJsonString:String = collection.FolderCache;
            if (folderJsonString != null)
            {
                folders.push(Utility.ParseJson(folderJsonString));
            }
            else
            {
                ready = false;

                if (collection.CacheStatus == 1)
                {
                    ThreadHelper.Run("CollectionProcessFolderCache", [collection.Id]);
                }
            }
        }

        if (!ready)
        {
            response.status = 1;
        }
        else
        {
            response.status = 0;
            response.folders = folders;
        }
        
        response.folders = folders;
        ajax.ReturnJson(response);
    }
}