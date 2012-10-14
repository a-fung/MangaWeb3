package afung.mangaWeb3.server;

import afung.mangaWeb3.common.CheckMySqlSettingRequest;
import afung.mangaWeb3.common.UserJson;

/**
 * ...
 * @author a-fung
 */

class User 
{
    public var Id(default, null):Int;
    
    public var Username(default, null):String;
    
    private var password:String;
    
    public var Admin(default, null):Bool;

    private function new() 
    {
        Id = -1;
    }
    
    public static function CreateNewUser(username:String, password:String, admin:Bool):User
    {
        var newUser:User = new User();
        newUser.Username = username;
        newUser.SetPassword(password);
        newUser.Admin = admin;
        
        return newUser;
    }
    
    private static function FromData(data:Hash<Dynamic>):User
    {
        var user:User = new User();
        user.Id = Std.parseInt(data.get("id"));
        user.Username = Std.string(data.get("username"));
        user.password = Std.string(data.get("password"));
        user.Admin = Std.parseInt(data.get("admin")) == 1;
        return user;
    }
    
    public static function GetCurrentUser(ajax:AjaxBase):User
    {
        return GetUser(SessionWrapper.GetUserName(ajax));
    }
    
    public static function GetUser(username:String, password:String = null):User
    {
        if (username != null && username != "")
        {
            var resultSet:Array<Hash<Dynamic>> = Database.Select("user", "`username`=" + Database.Quote(username) + (password == null ? "" : " AND `password`=" + Database.Quote(Utility.Md5(password))));
            
            if (resultSet.length == 1)
            {
                return FromData(resultSet[0]);
            }
        }
        
        return null;
    }
    
    public static function GetById(id:Int):User
    {
        var resultSet:Array<Hash<Dynamic>> = Database.Select("user", "`id`=" + Database.Quote(Std.string(id)));
        
        if (resultSet.length > 0)
        {
            return FromData(resultSet[0]);
        }
        
        return null;
    }
    
    public static function GetAllUsers():Array<User>
    {
        var resultSet:Array<Hash<Dynamic>> = Database.Select("user");
        var users:Array<User> = new Array<User>();
        
        for (result in resultSet)
        {
            users.push(FromData(result));
        }
        
        return users;
    }
    
    public static function IsAdminLoggedIn(ajax:AjaxBase):Bool
    {
        var currentUser:User = GetCurrentUser(ajax);
        return currentUser != null && currentUser.Admin;
    }
    
    public function SetPassword(password:String):Void
    {
        this.password = Utility.Md5(password);
    }
    
    public function MatchPassword(password:String):Bool
    {
        return this.password.toLowerCase() == password.toLowerCase();
    }
    
    public function Save():Void
    {
        var userData:Hash<Dynamic> = new Hash<Dynamic>();
        userData.set("username", Username);
        userData.set("password", password);
        userData.set("admin", Admin ? 1 : 0);

        if (Id == -1)
        {
            Id = Database.InsertAndReturnId("user", userData);
        }
        else
        {
            userData.set("id", Id);
            Database.Replace("user", userData);
        }
    }
    
    public function ToJson():UserJson
    {
        var obj:UserJson = new UserJson();
        obj.id = Id;
        obj.username = Username;
        obj.admin = Admin;
        return obj;
    }
    
    public static function ToJsonArray(users:Array<User>):Array<UserJson>
    {
        var objs:Array<UserJson> = new Array<UserJson>();
        for (user in users)
        {
            objs.push(user.ToJson());
        }
        
        return objs;
    }
    
    public static function DeleteUsers(ids:Array<Int>, currentUser:User):Void
    {
        if (currentUser != null && currentUser.Id != -1)
        {
            while (ids.remove(currentUser.Id))
            {
            }
        }
        
        Database.Delete("user", Database.BuildWhereClauseOr("id", ids));
        Database.Delete("collectionuser", Database.BuildWhereClauseOr("uid", ids));
    }
    
    public static function SetAdmin(ids:Array<Int>, admin:Bool, currentUser:User):Void
    {
        if (currentUser != null && currentUser.Id != -1)
        {
            while (ids.remove(currentUser.Id))
            {
            }
        }
        
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("admin", admin ? 1 : 0);
        Database.Update("user", data, Database.BuildWhereClauseOr("id", ids));
    }
}