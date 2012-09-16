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
	
	public static var UseZip(get_UseZip, set_UseZip):Bool;
	
	private static function get_UseZip():Bool
	{
		return GetSettings().get("use_zip") == "true";
	}
	
	private static function set_UseZip(value:Bool):Bool
	{
		SetSetting("use_zip", value ? "true" : "false");
		return value;
	}
	
	public static var UseRar(get_UseRar, set_UseRar):Bool;
	
	private static function get_UseRar():Bool
	{
		return GetSettings().get("use_rar") == "true";
	}
	
	private static function set_UseRar(value:Bool):Bool
	{
		SetSetting("use_rar", value ? "true" : "false");
		return value;
	}
	
	public static var UsePdf(get_UsePdf, set_UsePdf):Bool;
	
	private static function get_UsePdf():Bool
	{
		return GetSettings().get("use_pdf") == "true";
	}
	
	private static function set_UsePdf(value:Bool):Bool
	{
		SetSetting("use_pdf", value ? "true" : "false");
		return value;
	}
}