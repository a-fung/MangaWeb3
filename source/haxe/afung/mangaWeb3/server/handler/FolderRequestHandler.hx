package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.FolderJson;
import afung.mangaWeb3.common.FolderRequest;
import afung.mangaWeb3.common.FolderResponse;
import afung.mangaWeb3.server.Collection;

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
        var separator:String = "/";
        
        for (collection in collections)
        {
            var folder:FolderJson = new FolderJson();
            folder.name = collection.Name;
            folder.subfolders = [];
            folders.push(folder);
            var collectionPathLength:Int = collection.Path.length;

            var folderDictionary:Hash<FolderJson> = new Hash<FolderJson>();
            var resultSet:Array<Hash<Dynamic>> = Database.Select("manga", "`cid`=" + Database.Quote(Std.string(collection.Id)), null, null, "`path`");
            folderDictionary.set("", folder);
            
            for (result in resultSet)
            {
                var path:String = Std.string(result.get("path")).substr(collectionPathLength);
                var i:Int = 0, j:Int = 0;
                
                while ((i = path.indexOf(separator, j)) != -1)
                {
                    var relativePath = path.substr(0, i);
                    if (!folderDictionary.exists(relativePath))
                    {
                        var subfolder:FolderJson = new FolderJson();
                        subfolder.name = path.substr(j, i - j);
                        subfolder.subfolders = [];
                        folderDictionary.set(relativePath, subfolder);
                        
                        var k:Int;
                        var parentFolder:FolderJson = folderDictionary.get((k = relativePath.lastIndexOf(separator)) == -1 ? "" : relativePath.substr(0, k));
                        parentFolder.subfolders.push(subfolder);
                    }
                    
                    j = i + 1;
                }
            }
        }
        
        response.folders = folders;
        ajax.ReturnJson(response);
    }
}