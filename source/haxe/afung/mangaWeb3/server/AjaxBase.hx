package afung.mangaWeb3.server;

import afung.mangaWeb3.common.JsonRequest;
import afung.mangaWeb3.common.JsonResponse;
import afung.mangaWeb3.server.handler.HandlerBase;
import haxe.Json;
import php.Lib;
import php.Web;

/**
 * ...
 * @author a-fung
 */

class AjaxBase 
{
	public function new()
	{
	}
	
	public function Page_Load():Void
	{
		PageLoad();
	}
	
	public function HandleRequest(handlers:Array<HandlerBase>):Void
	{
		var jsonString:String = RequestParams("j");
		if (jsonString == null || jsonString == "" || StringTools.trim(jsonString) == "")
		{
			BadRequest();
			return;
		}
		
		var jsonRequest:JsonRequest = Utility.ParseJson(jsonString);
		if (jsonRequest == null || jsonRequest.type == null || StringTools.trim(jsonRequest.type) == "")
		{
			BadRequest();
			return;
		}
		
		for (handler in handlers)
		{
			if (handler.CanHandle(jsonRequest))
			{
				handler.HandleRequest(jsonString, this);
				return;
			}
		}
		
		BadRequest();
	}
	
	private function RequestParams(name:String):String
	{
		return Web.getParams().get(name);
	}
	
	public function BadRequest():Void
	{
		Web.setReturnCode(400);
		Web.setHeader("Cache-Control", "no-cache");
		Web.setHeader("Pragma", "no-cache");
		Lib.print("Bad Request");
		Web.flush();
	}
	
	public function Redirect(url:String):Void
	{
		Web.redirect(url);
	}
	
	public function ReturnJson(response:JsonResponse):Void
	{
		var output:String = Json.stringify(response);
		
		Web.setReturnCode(200);
		Web.setHeader("Cache-Control", "no-cache");
		Web.setHeader("Pragma", "no-cache");
		Web.setHeader("Content-Type", "application/json");
		Lib.print(output);
		Web.flush();
	}
	
	public function PageLoad():Void
	{
	}
}