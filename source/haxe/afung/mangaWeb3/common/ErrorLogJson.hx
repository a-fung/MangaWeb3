package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class ErrorLogJson extends JsonResponse
{
    public var id:Int;
    public var time:Int;
    public var type:String;
    public var source:String;
    public var message:String;
    public var stackTrace:String;
}