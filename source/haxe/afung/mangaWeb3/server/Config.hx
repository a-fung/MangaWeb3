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
	private static var ConfigExist(get_ConfigExist, null):Bool;
	
	private static function get_ConfigExist():Bool
	{
		return FileSystem.exists("config.php");
	}
	
	public static var IsInstalled(get_IsInstalled, null):Bool;
	
	public static function get_IsInstalled():Bool
	{
		if (!ConfigExist)
		{
			return false;
		}
		
		var installed:String = ConfigurationManagerAppSettings("MangaWebInstalled");
		return !(installed == null || StringTools.trim(installed) == "" || installed == "false");
	}
	
	public static var MySQLServer(get_MySQLServer, null):String;
	
	public static function get_MySQLServer():String
	{
		if (!ConfigExist)
		{
			return null;
		}
		
		return ConfigurationManagerAppSettings("MangaWebMySQLServer");
	}
	
	public static var MySQLPort(get_MySQLPort, null):Int;
	
	public static function get_MySQLPort():Int
	{
		if (!ConfigExist)
		{
			return 0;
		}
		
		var port:String = ConfigurationManagerAppSettings("MangaWebMySQLPort");
		return (port == null || StringTools.trim(port) == "") ? 0 : Std.parseInt(port);
	}
	
	public static var MySQLUser(get_MySQLUser, null):String;
	
	public static function get_MySQLUser():String
	{
		if (!ConfigExist)
		{
			return null;
		}
		
		return ConfigurationManagerAppSettings("MangaWebMySQLUser");
	}
	
	public static var MySQLPassword(get_MySQLPassword, null):String;
	
	public static function get_MySQLPassword():String
	{
		if (!ConfigExist)
		{
			return null;
		}
		
		return ConfigurationManagerAppSettings("MangaWebMySQLPassword");
	}
	
	public static var MySQLDatabase(get_MySQLDatabase, null):String;
	
	public static function get_MySQLDatabase():String
	{
		if (!ConfigExist)
		{
			return null;
		}
		
		return ConfigurationManagerAppSettings("MangaWebMySQLDatabase");
	}
	
	private static function ConfigurationManagerAppSettings(name:String):String
	{
		return Lib.hashOfAssociativeArray(ConfigurationManager.AppSettings()).get(name);
	}
}