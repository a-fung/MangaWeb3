package afung.mangaWeb3.server;

import afung.mangaWeb3.common.CollectionJson;
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
        return newCollection;
    }
    
    private static function FromData(data:Hash<Dynamic>):Collection
    {
        var collection:Collection = new Collection();
        collection.Id = Std.parseInt(data.get("id"));
        collection.Name = Std.string(data.get("name"));
        collection.Path = Std.string(data.get("path"));
        collection.Public = Std.parseInt(data.get("public")) == 1;
        collection.AutoAdd = Std.parseInt(data.get("autoadd")) == 1;
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
        var resultSet:Array<Hash<Dynamic>> = Database.Select("collection", "`id`=" + Database.Quote(Std.string(id)));
        
        if (resultSet.length > 0)
        {
            return FromData(resultSet[0]);
        }
        
        return null;
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

        if (Id == -1)
        {
            Database.Insert("collection", data);
            Id = Database.LastInsertId();
        }
        else
        {
            data.set("id", Id);
            Database.Replace("collection", data);
        }
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
        Database.Delete("collection", Database.BuildWhereClauseOr("id", ids));
        Database.Delete("collectionuser", Database.BuildWhereClauseOr("cid", ids));
        
        // todo: delete mangas
    }
    
    public static function SetCollectionsPublic(ids:Array<Int>, public_:Bool):Void
    {
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("public", public_ ? 1 : 0);
        Database.Update("collection", data, Database.BuildWhereClauseOr("id", ids));
    }
}