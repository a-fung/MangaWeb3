package afung.mangaWeb3.server;

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
    
    public static function SetUserName(ajax:AjaxBase, value:String)
    {
        if (value == null)
        {
            value = "";
        }

        Set(ajax, "username", value);
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