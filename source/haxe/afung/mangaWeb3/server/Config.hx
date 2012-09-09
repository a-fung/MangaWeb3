package afung.mangaWeb3.server;

import php.FileSystem;
import php.Lib;
import php.NativeArray;

/**
 * ...
 * @author a-fung
 */

class Config 
{
	public static var IsInstalled(get_IsInstalled, null):Bool;
	
	private static var ConfigExist(get_ConfigExist, null):Bool;
	
	public static function get_IsInstalled():Bool
	{
		if (!ConfigExist)
		{
			return false;
		}
		
		var installed:String = ConfigurationManagerAppSettings("MangaWebInstalled");
		return !(installed == null || StringTools.trim(installed) == "" || installed == "false");
	}
	
	private static function get_ConfigExist():Bool
	{
		return FileSystem.exists("config.php");
	}
	
	private static function ConfigurationManagerAppSettings(name:String):String
	{
		return Lib.hashOfAssociativeArray(ConfigurationManager.AppSettings()).get(name);
	}
}