package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class AdminErrorLogsGetResponse extends JsonResponse
{
    public var numberOfLogs:Int;
    public var logs:Array<ErrorLogJson>;
}