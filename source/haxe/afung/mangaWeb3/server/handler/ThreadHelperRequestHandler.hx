package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.ThreadHelperRequest;
import afung.mangaWeb3.server.ThreadHelper;
import afung.mangaWeb3.server.Utility;

/**
 * ...
 * @author a-fung
 */

class ThreadHelperRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return ThreadHelperRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (untyped __var__("_SERVER", "SERVER_ADDR") == untyped __var__("_SERVER", "REMOTE_ADDR"))
        {
            var request:ThreadHelperRequest = Utility.ParseJson(jsonString);
            ThreadHelper.InnerRun(request.methodName, request.parameters);
        }
    }
}