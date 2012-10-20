package afung.mangaWeb3.server.install.handler;

import afung.mangaWeb3.common.InstallRequest;
import afung.mangaWeb3.common.InstallResponse;
import afung.mangaWeb3.server.Database;
import afung.mangaWeb3.server.handler.HandlerBase;
import afung.mangaWeb3.server.Settings;
import afung.mangaWeb3.server.User;
import php.FileSystem;
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
        if (request.admin == null || request.admin == "" || regex.match(request.admin) || request.admin.length > 30 || request.password == null || request.password.length < 8 || request.password != request.password2)
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
        configFileContent.add("\"MangaWebMySQLUser\" => \"" + Native.AddSlashes(request.mysqlUser) + "\",");
        configFileContent.add("\"MangaWebMySQLPassword\" => \"" + Native.AddSlashes(request.mysqlPassword) + "\",");
        configFileContent.add("\"MangaWebMySQLDatabase\" => \"" + Native.AddSlashes(request.mysqlDatabase) + "\",");
        
        configFileContent.add("); } } ?>");
        
        File.saveContent("config.php", configFileContent.toString());

        // Import install.sql to MySQL
        var sqlFile:Array<String> = File.getContent("install.sql").split(";");
        for (sql in sqlFile)
        {
            if (StringTools.trim(sql) != "")
            {
                Database.ExecuteSql(sql);
            }
        }

        // Create Administrator
        User.CreateNewUser(request.admin, request.password, true).Save();

        // Save zip, rar, pdf to Settings table
        Settings.UseZip = request.zip;
        Settings.UseRar = request.rar;
        Settings.UsePdf = request.pdf;
        Settings.AllowGuest = true;
        Settings.LastAutoAddProcessTime = 0;
        Settings.MangaPagePreProcessCount = 5;
        Settings.MangaPagePreProcessDelay = 300;
        Settings.MangaCacheSizeLimit = 200;
        
        // Delete Install files
        Native.Exec("rm -f ./install.html");
        Native.Exec("rm -f ./install.sql");
        Native.Exec("rm -f ./template/install.html");
        Native.Exec("rm -f ./InstallAjax.php");
        Native.Exec("rm -f ./js/afung.MangaWeb3.Client.Install.*");
        Native.Exec("rm -r -f ./lib/afung/mangaWeb3/server/install");
        
        var response:InstallResponse = new InstallResponse();
        response.installsuccessful = true;

        ajax.ReturnJson(response);
    }
}