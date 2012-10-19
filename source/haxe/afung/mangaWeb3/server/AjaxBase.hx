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
        try
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
        catch (ex:Dynamic)
        {
            Utility.TryLogError(ex);
        }
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
    }
    
    public function Unauthorized():Void
    {
        Web.setReturnCode(401);
        Web.setHeader("Cache-Control", "no-cache");
        Web.setHeader("Pragma", "no-cache");
        Lib.print("Unauthorized");
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
    }
    
    public function PageLoad():Void
    {
    }
}