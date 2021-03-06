package afung.mangaWeb3.server;

/**
 * ...
 * @author a-fung
 */

class Settings 
{
    private static var settings:Hash<String> = null;

    private static function GetSettings():Hash<String>
    {
        if (settings == null)
        {
            settings = new Hash<String>();
            var results:Array<Hash<Dynamic>> = Database.Select("setting");
            var i:Int = 0;
            while (i < results.length)
            {
                settings.set(results[i].get("name"), results[i].get("value"));
                i++;
            }
        }
        return settings;
    }
        
    private static function SetSetting(name:String, value:String):Void
    {
        GetSettings();
        settings.set(name, value);
        
        var data:Hash<String> = new Hash<String>();
        data.set("name", name);
        data.set("value", value);
        Database.Replace("setting", data);
    }
    
    public static var AllowGuest(get_AllowGuest, set_AllowGuest):Bool;
    
    private static function get_AllowGuest():Bool
    {
        return GetSettings().get("allow_guest") != "false";
    }
    
    private static function set_AllowGuest(value:Bool):Bool
    {
        SetSetting("allow_guest", value ? "true" : "false");
        return value;
    }
    
    public static var UseZip(get_UseZip, set_UseZip):Bool;
    
    private static function get_UseZip():Bool
    {
        return GetSettings().get("use_zip") == "true" && Native.ExtensionLoaded("zip");
    }
    
    private static function set_UseZip(value:Bool):Bool
    {
        SetSetting("use_zip", value ? "true" : "false");
        return value;
    }
    
    public static var UseRar(get_UseRar, set_UseRar):Bool;
    
    private static function get_UseRar():Bool
    {
        return GetSettings().get("use_rar") == "true" && Native.ClassExists("RarArchive");
    }
    
    private static function set_UseRar(value:Bool):Bool
    {
        SetSetting("use_rar", value ? "true" : "false");
        return value;
    }
    
    public static var UsePdf(get_UsePdf, set_UsePdf):Bool;
    
    private static var _usePdf:Null<Bool> = null;
    
    private static function get_UsePdf():Bool
    {
        if (_usePdf == null)
        {
            _usePdf = GetSettings().get("use_pdf") == "true";
            
            if (_usePdf)
            {
                Native.Exec("pdfinfo empty.pdf");
                if (0 != Native.ExecReturnVar)
                {
                    return _usePdf = false;
                }
                
                Native.Exec("pdfdraw empty.pdf");
                if (0 != Native.ExecReturnVar)
                {
                    return _usePdf = false;
                }
            }
        }
        
        return _usePdf;
    }
    
    private static function set_UsePdf(value:Bool):Bool
    {
        _usePdf = null;
        SetSetting("use_pdf", value ? "true" : "false");
        return value;
    }
    
    public static var LastAutoAddProcessTime(get_LastAutoAddProcessTime, set_LastAutoAddProcessTime):Int;
    
    private static function get_LastAutoAddProcessTime():Int
    {
        return Std.parseInt(GetSettings().get("last_autoadd_time"));
    }
    
    private static function set_LastAutoAddProcessTime(value:Int):Int
    {
        SetSetting("last_autoadd_time", Std.string(value));
        return value;
    }
    
    public static var MangaPagePreProcessCount(get_MangaPagePreProcessCount, set_MangaPagePreProcessCount):Int;
    
    private static function get_MangaPagePreProcessCount():Int
    {
        return Std.parseInt(GetSettings().get("mangapage_preprocess_count"));
    }
    
    private static function set_MangaPagePreProcessCount(value:Int):Int
    {
        SetSetting("mangapage_preprocess_count", Std.string(value));
        return value;
    }
    
    public static var MangaPagePreProcessDelay(get_MangaPagePreProcessDelay, set_MangaPagePreProcessDelay):Int;
    
    private static function get_MangaPagePreProcessDelay():Int
    {
        return Std.parseInt(GetSettings().get("mangapage_preprocess_delay"));
    }
    
    private static function set_MangaPagePreProcessDelay(value:Int):Int
    {
        SetSetting("mangapage_preprocess_delay", Std.string(value));
        return value;
    }
    
    public static var MangaCacheSizeLimit(get_MangaCacheSizeLimit, set_MangaCacheSizeLimit):Int;
    
    private static function get_MangaCacheSizeLimit():Int
    {
        return Std.parseInt(GetSettings().get("mangacache_sizelimit"));
    }
    
    private static function set_MangaCacheSizeLimit(value:Int):Int
    {
        SetSetting("mangacache_sizelimit", Std.string(value));
        return value;
    }
}