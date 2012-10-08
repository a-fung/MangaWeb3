package afung.mangaWeb3.server;

import afung.mangaWeb3.common.ThreadHelperRequest;
import haxe.Json;
import php.Web;

/**
 * ...
 * @author a-fung
 */

class ThreadHelper 
{
    public static function Run(methodName:String, parameters:Array<Dynamic>):Void
    {
        var request:ThreadHelperRequest = new ThreadHelperRequest();
        request.methodName = methodName;
        request.parameters = parameters;
        
        var jsonRequestString = Json.stringify(request);
        
        var errno:Int = 0;
        var errstr:String = "";
        var port:Int = untyped __var__("_SERVER", "SERVER_PORT");
        var host:String = (port == 443 ? "ssl://" : "") + untyped __var__("_SERVER", "SERVER_ADDR");
        var sckt:Dynamic = untyped __call__("fsockopen", host, port, errno, errstr, 30);
        var post_string:String = "j=" + untyped __call__("urlencode", jsonRequestString);
        
        var out:String = "POST " + untyped __var__("_SERVER", "REQUEST_URI") + " HTTP/1.1\r\n";
        out += "Host: " + untyped __var__("_SERVER", "SERVER_NAME") + "\r\n";
        out += "Content-Type: application/x-www-form-urlencoded\r\n";
        out += "Content-Length: " + Std.string(post_string.length) + "\r\n";
        out += "Connection: Close\r\n\r\n";
        out += post_string;

        untyped __call__("fwrite", sckt, out);
        untyped __call__("fclose", sckt);
    }
    
    public static function InnerRun(methodName:String, parameters:Array<Dynamic>):Void
    {
        switch(methodName)
        {
            case "MangaProcessFile":
                MangaProcessFile(parameters);
            default:
                return;
        }
    }
    
    private static function MangaProcessFile(parameters:Array<Dynamic>):Void
    {
        var id:Int = parameters[0];
        var manga:Manga = Manga.GetById(id);
        
        if (manga != null)
        {
            manga.ProcessFile(parameters[1], parameters[2], parameters[3], parameters[4], parameters[5]);
        }
    }
}