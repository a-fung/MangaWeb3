package afung.mangaWeb3.server.install.handler;

import afung.mangaWeb3.common.CheckMySqlSettingRequest;
import afung.mangaWeb3.common.CheckMySqlSettingResponse;
import afung.mangaWeb3.server.Database;
import afung.mangaWeb3.server.handler.HandlerBase;
import afung.mangaWeb3.server.Utility;
import php.db.Connection;

/**
 * ...
 * @author a-fung
 */

class CheckMySqlSettingRequestHandler extends HandlerBase
{
	public override function GetHandleRequestType():Class<Dynamic>
	{
		return CheckMySqlSettingRequest;
	}
	
	public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
	{
		var request:CheckMySqlSettingRequest = Utility.ParseJson(jsonString);
		
		var connection:Connection = null;
		var pass:Bool = false;
		try
		{
			connection = Database.GetConnnection(
				request.server,
				request.port,
				request.username,
				request.password,
				request.database);
				
			pass = true;
		}
		catch (e:Dynamic)
		{
		}
		
		if (connection != null)
		{
			connection.close();
		}
		
		var response:CheckMySqlSettingResponse = new CheckMySqlSettingResponse();
		response.pass = pass;
		
		ajax.ReturnJson(response);
	}
}