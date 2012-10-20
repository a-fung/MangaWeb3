package afung.mangaWeb3.server;

import afung.mangaWeb3.common.CollectionJson;
import afung.mangaWeb3.common.FolderJson;
import haxe.Json;
import php.FileSystem;

/**
 * ...
 * @author a-fung
 */

class Collection 
{
    public var Id(default, null):Int;
    
    public var Name(default, default):String;
    
    public var Path(default, null):String;
    
    public var Public(default, null):Bool;
    
    public var AutoAdd(default, null):Bool;
    
    public var CacheStatus(default, null):Int;
    
    private var _folderCache:String;
    
    public var FolderCache(get_FolderCache, never):String;
    
    private function get_FolderCache():String
    {
        if (CacheStatus == 0)
        {
            if (_folderCache == null)
            {
                _folderCache = Std.string(Database.Select("foldercache", "`id`='" + Id + "'")[0].get("content"));
            }

            return _folderCache;
        }

        return null;
    }
    
    private static var cache:IntHash<Collection> = new IntHash<Collection>();

    private function new()
    {
        Id = -1;
    }
    
    public static function CreateNewCollection(name:String, path:String, public_:Bool, autoadd:Bool):Collection
    {
        var newCollection:Collection = new Collection();
        newCollection.Name = name;
        newCollection.Path = path;
        newCollection.Public = public_;
        newCollection.AutoAdd = autoadd;
        newCollection.CacheStatus = 1;
        return newCollection;
    }
    
    private static function FromData(data:Hash<Dynamic>):Collection
    {
        var id:Int = Std.parseInt(data.get("id"));
        var collection:Collection;
        if ((collection = cache.get(id)) != null)
        {
            return collection;
        }
        
        collection = new Collection();
        collection.Id = id;
        collection.Name = Std.string(data.get("name"));
        collection.Path = Std.string(data.get("path"));
        collection.Public = Std.parseInt(data.get("public")) == 1;
        collection.AutoAdd = Std.parseInt(data.get("autoadd")) == 1;
        collection.CacheStatus = Std.parseInt(data.get("cachestatus"));
        
        cache.set(collection.Id, collection);
        return collection;
    }
    
    public static function GetByName(name:String):Collection
    {
        if (name != null && name != "")
        {
            var resultSet:Array<Hash<Dynamic>> = Database.Select("collection", "`name`=" + Database.Quote(name));
            
            if (resultSet.length > 0)
            {
                return FromData(resultSet[0]);
            }
        }
        
        return null;
    }
    
    public static function GetById(id:Int):Collection
    {
        if (cache.exists(id))
        {
            return cache.get(id);
        }
        
        var resultSet:Array<Hash<Dynamic>> = Database.Select("collection", "`id`=" + Database.Quote(Std.string(id)));
        
        if (resultSet.length > 0)
        {
            return FromData(resultSet[0]);
        }
        
        return null;
    }
    
    public static function GetAccessible(ajax:AjaxBase):Array<Collection>
    {
        var user:User = User.GetCurrentUser(ajax);
        var where:String = "FALSE";
        if (Settings.AllowGuest || user != null)
        {
            where += " OR `public`='1'";
        }

        if (user != null)
        {
            where += " OR `id` IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(Std.string(user.Id)) + " AND `access`='1')";
        }
        
        var resultSet:Array<Hash<Dynamic>> = Database.Select("collection", where);
        var collections:Array<Collection> = new Array<Collection>();
        
        for (result in resultSet)
        {
            collections.push(FromData(result));
        }
        
        return collections;
    }
    
    public static function GetAutoAdd():Array<Collection>
    {
        var resultSet:Array<Hash<Dynamic>> = Database.Select("collection", "`autoadd`='1'");
        var collections:Array<Collection> = new Array<Collection>();
        
        for (result in resultSet)
        {
            collections.push(FromData(result));
        }
        
        return collections;
    }
    
    public static function GetAllCollections():Array<Collection>
    {
        var resultSet:Array<Hash<Dynamic>> = Database.Select("collection");
        var collections:Array<Collection> = new Array<Collection>();
        
        for (result in resultSet)
        {
            collections.push(FromData(result));
        }
        
        return collections;
    }
    
    public static function GetAllCollectionNames():Array<String>
    {
        return Database.GetDistinctStringValues("collection", "name");
    }
    
    public static function CheckNewCollectionName(name:String):Bool
    {
        return GetByName(name) == null;
    }
    
    public static function CheckNewCollectionPath(path:String):String
    {
        path = FileSystem.fullPath(path);
        
        if (path == null || !FileSystem.exists(path) || !FileSystem.isDirectory(path))
        {
            return null;
        }
        
        if (path.charAt(path.length - 1) != "/")
        {
            path += "/";
        }
        
        var collections:Array<Collection> = GetAllCollections();
        
        for (collection in collections)
        {
            if (path.indexOf(collection.Path) == 0 || collection.Path.indexOf(path) == 0)
            {
                return null;
            }
        }
        
        return path;
    }
    
    public function Save():Void
    {
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("name", Name);
        data.set("path", Path);
        data.set("public", Public ? 1 : 0);
        data.set("autoadd", AutoAdd ? 1 : 0);
        data.set("cachestatus", CacheStatus);

        if (Id == -1)
        {
            Id = Database.InsertAndReturnId("collection", data);
        }
        else
        {
            data.set("id", Id);
            Database.Replace("collection", data);
        }
        
        cache.set(Id, this);
    }
    
    public function ToJson():CollectionJson
    {
        var obj:CollectionJson = new CollectionJson();
        obj.id = Id;
        obj.name = Name;
        obj.path = Path;
        obj.public_ = Public;
        obj.autoadd = AutoAdd;
        return obj;
    }
    
    public static function ToJsonArray(collections:Array<Collection>):Array<CollectionJson>
    {
        var objs:Array<CollectionJson> = new Array<CollectionJson>();
        for (collection in collections)
        {
            objs.push(collection.ToJson());
        }
        
        return objs;
    }
    
    public static function DeleteCollections(ids:Array<Int>):Void
    {
        Manga.DeleteMangasFromCollectionIds(ids);
        Database.Delete("collection", Database.BuildWhereClauseOr("id", ids));
        Database.Delete("collectionuser", Database.BuildWhereClauseOr("cid", ids));
    }
    
    public static function SetCollectionsPublic(ids:Array<Int>, public_:Bool):Void
    {
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("public", public_ ? 1 : 0);
        Database.Update("collection", data, Database.BuildWhereClauseOr("id", ids));
    }
    
    public function Accessible(ajax:AjaxBase):Bool
    {
        var where:String = "`id`=" + Database.Quote(Std.string(Id));
        var user:User = User.GetCurrentUser(ajax);
        var collectionSelect:String = "FALSE";
        if (Settings.AllowGuest || user != null)
        {
            collectionSelect += " OR `public`='1'";
        }

        if (user != null)
        {
            collectionSelect += " OR `id` IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(Std.string(user.Id)) + " AND `access`='1')";
            where += " AND `id` NOT IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(Std.string(user.Id)) + " AND `access`='0')";
        }

        where += " AND (" + collectionSelect + ")";

        return Database.Select("collection", where, null, null, "`id`").length > 0;
    }
    
    public function MarkFolderCacheDirty():Void
    {
        CacheStatus = 1;
        Save();
    }
    
    public function ProcessFolderCache():Void
    {
        if (CacheStatus == 0 || CacheStatus == 2)
        {
            return;
        }

        CacheStatus = 2;
        Save();
        
        var folder:FolderJson = new FolderJson();
        folder.name = Name;
        folder.subfolders = [];
        var collectionPathLength:Int = Path.length;
        var separator:String = "/";

        var folderDictionary:Hash<FolderJson> = new Hash<FolderJson>();
        var resultSet:Array<Hash<Dynamic>> = Database.Select("manga", "`cid`=" + Database.Quote(Std.string(Id)), null, null, "`path`");
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
        
        var cacheData:Hash<Dynamic> = new Hash<Dynamic>();
        cacheData.set("id", Id);
        cacheData.set("content", _folderCache = Json.stringify(folder));
        Database.Replace("foldercache", cacheData);

        CacheStatus = 0;
        Save();
    }
}