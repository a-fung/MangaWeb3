package afung.mangaWeb3.server.handler;
import afung.mangaWeb3.common.JsonRequest;
import afung.mangaWeb3.server.AjaxBase;

/**
 * ...
 * @author a-fung
 */

class HandlerBase 
{
	public function new() 
	{
	}
	
	public function CanHandle(jsonRequest:JsonRequest):Bool
	{
		var fullName:String = Type.getClassName(GetHandleRequestType());
		var name:String = fullName.substr(fullName.lastIndexOf(".") + 1);
		return jsonRequest.type == name;
	}
	
	public function GetHandleRequestType():Class<Dynamic>
	{
		return JsonRequest;
	}
	
	public function HandleRequest(jsonString:String, ajax:AjaxBase):Void
	{
	}
}