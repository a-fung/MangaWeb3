package afung.mangaWeb3.server;

import php.Lib;
import php.NativeArray;
import php.Session;


/**
 * ...
 * @author a-fung
 */

class SessionWrapper 
{
    public static function GetUserName(ajax:AjaxBase):String
    {
        var username:Dynamic = Get(ajax, "username");
        
        if (username == null)
        {
            return "";
        }
        else
        {
            return Std.string(username);
        }
    }
    
    public static function SetUserName(ajax:AjaxBase, value:String):Void
    {
        if (value == null)
        {
            value = "";
        }

        Set(ajax, "username", value);
    }
    
    public static function SetFinderData(ajax:AjaxBase, token:String, finderData:NativeArray):Void
    {
        var array:NativeArray = Get(ajax, "finder");
        if (array == null)
        {
            array = Lib.toPhpArray([]);
        }
        
        var dict:Hash<NativeArray> = Lib.hashOfAssociativeArray(array);
        dict.set(token, finderData);
        
        Set(ajax, "finder", Lib.associativeArrayOfHash(dict));
    }
    
    private static function Get(ajax:AjaxBase, name:String):Dynamic
    {
        return Session.get("afung.MangaWeb3.Server.Session." + name);
    }
    
    private static function Set(ajax:AjaxBase, name:String, value:Dynamic):Void
    {
        Session.set("afung.MangaWeb3.Server.Session." + name, value);
    }
}