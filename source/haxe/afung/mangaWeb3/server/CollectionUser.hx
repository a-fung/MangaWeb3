package afung.mangaWeb3.server;
import afung.mangaWeb3.common.CollectionUserJson;

/**
 * ...
 * @author a-fung
 */

class CollectionUser 
{
    private var isNew:Bool = false;
    
    public var CollectionId(default, null):Int;
    
    public var UserId(default, null):Int;
    
    public var Access(default, null):Bool;

    private function new() 
    {
    }
    
    public static function CreateNew(collection:Collection, user:User, access:Bool):CollectionUser
    {
        var cu:CollectionUser = new CollectionUser();
        cu.CollectionId = collection.Id;
        cu.UserId = user.Id;
        cu.Access = access;
        cu.isNew = true;
        return cu;
    }
    
    private static function FromData(data:Hash<Dynamic>):CollectionUser
    {
        var cu:CollectionUser = new CollectionUser();
        cu.CollectionId = Std.parseInt(data.get("cid"));
        cu.UserId = Std.parseInt(data.get("uid"));
        cu.Access = Std.parseInt(data.get("access")) == 1;
        return cu;
    }
    
    public static function GetByCollection(collection:Collection):Array<CollectionUser>
    {
        if (collection != null && collection.Id != -1)
        {
            return GetMultiple("`cid`=" + Database.Quote(Std.string(collection.Id)));
        }
        
        return new Array<CollectionUser>();
    }
    
    public static function GetByUser(user:User):Array<CollectionUser>
    {
        if (user != null && user.Id != -1)
        {
            return GetMultiple("`uid`=" + Database.Quote(Std.string(user.Id)));
        }
        
        return new Array<CollectionUser>();
    }
    
    private static function GetMultiple(where:String):Array<CollectionUser>
    {
        var resultSet:Array<Hash<Dynamic>> = Database.Select("collectionuser", where);
        var cus:Array<CollectionUser> = new Array<CollectionUser>();
        
        for (result in resultSet)
        {
            cus.push(FromData(result));
        }
        
        return cus;
    }
    
    public static function Get(collection:Collection, user:User):CollectionUser
    {
        var cus:Array<CollectionUser> = GetMultiple("`cid`=" + Database.Quote(Std.string(collection.Id)) + " AND `uid`=" + Database.Quote(Std.string(user.Id)));
        if (cus.length > 0)
        {
            return cus[0];
        }

        return null;
    }
    
    public function Save():Void
    {
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("cid", CollectionId);
        data.set("uid", UserId);
        data.set("access", Access ? 1 : 0);

        if (isNew)
        {
            Database.Insert("collectionuser", data);
        }
        else
        {
            Database.Update("collectionuser", data, "`cid`=" + Database.Quote(Std.string(CollectionId)) + " AND `uid`=" + Database.Quote(Std.string(UserId)));
        }
    }
    
    public function ToJson():CollectionUserJson
    {
        var obj:CollectionUserJson = new CollectionUserJson();
        obj.cid = CollectionId;
        obj.uid = UserId;
        obj.access = Access;
        obj.collectionName = Collection.GetById(CollectionId).Name;
        obj.username = User.GetById(UserId).Username;
        return obj;
    }
    
    public static function ToJsonArray(cus:Array<CollectionUser>):Array<CollectionUserJson>
    {
        var objs:Array<CollectionUserJson> = new Array<CollectionUserJson>();
        for (cu in cus)
        {
            objs.push(cu.ToJson());
        }
        
        return objs;
    }
    
    public static function DeleteRelations(t:Int, id:Int, ids:Array<Int>):Void
    {
        var primaryId:String = t == 0 ? "cid" : "uid";
        var secondaryId:String = t == 0 ? "uid" : "cid";

        Database.Delete("collectionuser", "`" + primaryId + "`=" + Database.Quote(Std.string(id)) + " AND " + Database.BuildWhereClauseOr(secondaryId, ids));
    }
    
    public static function SetRelationsAccess(t:Int, id:Int, ids:Array<Int>, access:Bool):Void
    {
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("access", access ? 1 : 0);
        
        var primaryId:String = t == 0 ? "cid" : "uid";
        var secondaryId:String = t == 0 ? "uid" : "cid";

        Database.Update("collectionuser", data, "`" + primaryId + "`=" + Database.Quote(Std.string(id)) + " AND " + Database.BuildWhereClauseOr(secondaryId, ids));
    }
}