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
}