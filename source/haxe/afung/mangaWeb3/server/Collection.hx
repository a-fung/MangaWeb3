// Collection.hx
// MangaWeb3 Project
// Copyright 2012 Man Kwan Liu

package afung.mangaWeb3.server;

import afung.mangaWeb3.common.CollectionJson;
import afung.mangaWeb3.common.FolderJson;
import haxe.Json;
import php.FileSystem;

/**
 * ...
 * @author a-fung
 */

/// <summary>
/// The Collection class
/// </summary>
class Collection 
{
    /// <summary>
    /// ID of the collection
    /// </summary>
    public var Id(default, null):Int;
    
    /// <summary>
    /// Name of the collection
    /// </summary>
    public var Name(default, default):String;
    
    /// <summary>
    /// Path of the collection
    /// </summary>
    public var Path(default, null):String;
    
    /// <summary>
    /// Whether the collection is public
    /// </summary>
    public var Public(default, null):Bool;
    
    /// <summary>
    /// Whether the collection uses auto add
    /// </summary>
    public var AutoAdd(default, null):Bool;
    
    /// <summary>
    /// The folder cache status of the collection
    /// </summary>
    public var CacheStatus(default, null):Int;
    
    /// <summary>
    /// The folder cache in json string
    /// </summary>
    private var _folderCache:String;
    
    /// <summary>
    /// Getter the folder cache
    /// </summary>
    public var FolderCache(get_FolderCache, never):String;
    
    /// <summary>
    /// Getter function of the folder cache
    /// </summary>
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
    
    /// <summary>
    /// The cache of collections
    /// </summary>
    private static var cache:IntHash<Collection> = new IntHash<Collection>();

    /// <summary>
    /// Instantiate a new instance of Collection class
    /// </summary>
    private function new()
    {
        Id = -1;
    }
    
    /// <summary>
    /// Create a new collection
    /// </summary>
    /// <param name="name">The collection name</param>
    /// <param name="path">The collection path</param>
    /// <param name="public_">Whether the collection is public</param>
    /// <param name="autoadd">Whether the collection uses auto add</param>
    /// <returns>A new collection</returns>
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
    
    /// <summary>
    /// Create a new instance of Collection using data from database
    /// </summary>
    /// <param name="data">The data</param>
    /// <returns>A collection from data</returns>
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
    
    /// <summary>
    /// Get a collection from database by name
    /// </summary>
    /// <param name="name">The name</param>
    /// <returns>A collection object or null if not found</returns>
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
    
    /// <summary>
    /// Get a collection from database by ID
    /// </summary>
    /// <param name="name">The ID</param>
    /// <returns>A collection object or null if not found</returns>
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
    
    /// <summary>
    /// Get a list of collections from database which is accessible by the current user
    /// </summary>
    /// <param name="ajax">The AjaxBase object which received the request</param>
    /// <returns>A list of collections</returns>
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
            where = "(" + where + ") AND `id` NOT IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(Std.string(user.Id)) + " AND `access`='0')";
        }
        
        var resultSet:Array<Hash<Dynamic>> = Database.Select("collection", where);
        var collections:Array<Collection> = new Array<Collection>();
        
        for (result in resultSet)
        {
            collections.push(FromData(result));
        }
        
        return collections;
    }
    
    /// <summary>
    /// Get a list of collections from database which uses auto add
    /// </summary>
    /// <returns>A list of collections</returns>
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

    /// <summary>
    /// Get all collections from database
    /// </summary>
    /// <returns>All collections</returns>
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

    /// <summary>
    /// Get the names of all the collections from database
    /// </summary>
    /// <returns>An array of string containing all the names</returns>
    public static function GetAllCollectionNames():Array<String>
    {
        return Database.GetDistinctStringValues("collection", "name");
    }
    
    /// <summary>
    /// Check whether a name can be used for creating a new collection
    /// </summary>
    /// <param name="name">The name</param>
    /// <returns>True if the name is valid for new collection</returns>
    public static function CheckNewCollectionName(name:String):Bool
    {
        return GetByName(name) == null;
    }

    /// <summary>
    /// Check whether a path can be used for creating a new collection
    /// </summary>
    /// <param name="name">The path (absolute or relative)</param>
    /// <returns>null if the path is invalid, a normalized absolute path if the path is valid</returns>
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
    
    /// <summary>
    /// Save the collection to database
    /// </summary>
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

    /// <summary>
    /// Get an object to be stringified and passed to client app
    /// </summary>
    /// <returns>A CollectionJson object</returns>
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

    /// <summary>
    /// Get an array of CollectionJson to be passed to client app
    /// </summary>
    /// <param name="collections">An array of collections</param>
    /// <returns>An array of CollectionJson</returns>
    public static function ToJsonArray(collections:Array<Collection>):Array<CollectionJson>
    {
        var objs:Array<CollectionJson> = new Array<CollectionJson>();
        for (collection in collections)
        {
            objs.push(collection.ToJson());
        }
        
        return objs;
    }
    
    /// <summary>
    /// Delete collections and all associated data from database
    /// </summary>
    /// <param name="ids">A list of collection IDs</param>
    public static function DeleteCollections(ids:Array<Int>):Void
    {
        Manga.DeleteMangasFromCollectionIds(ids);
        Database.Delete("collection", Database.BuildWhereClauseOr("id", ids));
        Database.Delete("collectionuser", Database.BuildWhereClauseOr("cid", ids));
    }

    /// <summary>
    /// Set collections to be public/private
    /// </summary>
    /// <param name="ids">A list of collection IDs</param>
    /// <param name="public_">Whether is public or private</param>
    public static function SetCollectionsPublic(ids:Array<Int>, public_:Bool):Void
    {
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("public", public_ ? 1 : 0);
        Database.Update("collection", data, Database.BuildWhereClauseOr("id", ids));
    }
    
    /// <summary>
    /// Get whether the current request can access the collection
    /// </summary>
    /// <param name="ajax">The AjaxBase object which received the request</param>
    /// <returns>True if accessible</returns>
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

    /// <summary>
    /// Mark Folder Cache Dirty
    /// </summary>
    public function MarkFolderCacheDirty():Void
    {
        CacheStatus = 1;
        Save();
    }
    
    /// <summary>
    /// Process folder cache and save it in database
    /// </summary>
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
            var lastFolderPath:String = null;
            var i:Int = 0, j:Int = 0;
            
            while ((i = path.indexOf(separator, j)) != -1)
            {
                var relativePath:String = lastFolderPath = path.substr(0, i);
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
            
            if (lastFolderPath != null)
            {
                folderDictionary.get(lastFolderPath).count++;
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