// AjaxBase.hx
// MangaWeb3 Project
// Copyright 2012 Man Kwan Liu

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

/// <summary>
/// The base class for Http request server entry point
/// </summary>
class AjaxBase 
{
    /// <summary>
    /// Instantiate a new instance of AjaxBase
    /// </summary>
    public function new()
    {
    }
    
    /// <summary>
    /// Called when the page loads
    /// </summary>
    public function Page_Load():Void
    {
        PageLoad();
    }
    
    /// <summary>
    /// Handle the json request using a list of handlers
    /// </summary>
    /// <param name="handlers">The array of handlers</param>
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
            throw ex;
        }
    }
    
    /// <summary>
    /// Get the value of a request parameter
    /// </summary>
    /// <param name="name">The parameter name</param>
    /// <returns>The value</returns>
    private function RequestParams(name:String):String
    {
        return Web.getParams().get(name);
    }
    
    /// <summary>
    /// Return "bad request" to the client app
    /// </summary>
    public function BadRequest():Void
    {
        Web.setReturnCode(400);
        Web.setHeader("Cache-Control", "no-cache");
        Web.setHeader("Pragma", "no-cache");
        Lib.print("Bad Request");
    }
    
    /// <summary>
    /// Return "unauthorized" to the client app
    /// </summary>
    public function Unauthorized():Void
    {
        Web.setReturnCode(401);
        Web.setHeader("Cache-Control", "no-cache");
        Web.setHeader("Pragma", "no-cache");
        Lib.print("Unauthorized");
    }
    
    /// <summary>
    /// Return a Json response to the client app
    /// </summary>
    /// <param name="response">The response object to be stringify and return to client app</param>
    public function ReturnJson(response:JsonResponse):Void
    {
        var output:String = Json.stringify(response);
        
        Web.setReturnCode(200);
        Web.setHeader("Cache-Control", "no-cache");
        Web.setHeader("Pragma", "no-cache");
        Web.setHeader("Content-Type", "application/json");
        Lib.print(output);
    }
    
    /// <summary>
    /// The PageLoad function is to be overriden by child class
    /// In this function it should create a list of handlers and call the HandleRequest function
    /// </summary>
    public function PageLoad():Void
    {
    }
}