package afung.mangaWeb3.server.install.handler;

import afung.mangaWeb3.common.InstallRequest;
import afung.mangaWeb3.server.Database;
import afung.mangaWeb3.server.handler.HandlerBase;
import php.io.File;

/**
 * ...
 * @author a-fung
 */

class InstallRequestHandler extends HandlerBase 
{
	public override function GetHandleRequestType():Class<Dynamic>
	{
		return InstallRequest;
	}
	
	public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
	{
		var request:InstallRequest = Utility.ParseJson(jsonString);

		// Check Admin user name and password
		var regex:EReg = ~/[^a-zA-Z0-9]/;
		if (request.admin == null || request.admin == "" || regex.match(request.admin) || request.password == null || request.password.length < 8 || request.password != request.password2)
		{
			ajax.BadRequest();
			return;
		}

		// Save MySQL setting to config.php
		var configFileContent:StringBuf = new StringBuf();
		configFileContent.add("<?php class ConfigurationManager { public function __construct() {} public static function AppSettings() { return array(");
		
		configFileContent.add("\"MangaWebInstalled\" => \"true\",");
		
		configFileContent.add("\"MangaWebMySQLServer\" => \"" + Native.AddSlashes(request.mysqlServer) + "\",");
		configFileContent.add("\"MangaWebMySQLPort\" => \"" + request.mysqlPort + "\",");
		configFileContent.add("\"MangaWebMySQLUser\" => \"" + Native.AddSlashes(request.admin) + "\",");
		configFileContent.add("\"MangaWebMySQLPassword\" => \"" + Native.AddSlashes(request.mysqlPassword) + "\",");
		configFileContent.add("\"MangaWebMySQLDatabase\" => \"" + Native.AddSlashes(request.mysqlDatabase) + "\",");
		
		configFileContent.add("); } } ?>");
		
		File.saveContent("config.php", configFileContent.toString());

		// Import install.sql to MySQL
		Database.ExecuteSql(File.getContent("install.sql"));

		// Create Administrator

		// Save zip, rar, pdf to Settings table
		
	}
}