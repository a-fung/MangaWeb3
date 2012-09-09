package afung.mangaWeb3.server.install.handler;

import afung.mangaWeb3.common.PreInstallCheckRequest;
import afung.mangaWeb3.common.PreInstallCheckResponse;
import afung.mangaWeb3.server.AjaxBase;
import afung.mangaWeb3.server.Config;
import afung.mangaWeb3.server.handler.HandlerBase;
import afung.mangaWeb3.server.Utility;
import php.Lib;

/**
 * ...
 * @author a-fung
 */

class PreInstallCheckRequestHandler extends HandlerBase
{
	public override function GetHandleRequestType():Class<Dynamic>
	{
		return PreInstallCheckRequest;
	}
	
	public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
	{
		var response:PreInstallCheckResponse = new PreInstallCheckResponse();
		response.installed = Config.IsInstalled;
		
		if (!response.installed)
		{
			response.mySql = Native.ExtensionLoaded("mysql");
			response.gd = Native.ExtensionLoaded("gd");
			response.zip = Native.ExtensionLoaded("zip");
			response.rar = Native.ClassExists("RarArchive");
			
			Native.Exec("pdfinfo");
			response.pdfinfo = 127 != Native.ExecReturnVar;
			
			Native.Exec("pdfdraw");
			response.pdfdraw = 127 != Native.ExecReturnVar;
		}
		
		ajax.ReturnJson(response);
	}
}