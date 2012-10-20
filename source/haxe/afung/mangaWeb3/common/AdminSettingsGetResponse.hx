package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class AdminSettingsGetResponse extends JsonResponse
{
    public var guest:Bool;
    public var zip:Bool;
    public var rar:Bool;
    public var pdf:Bool;
    public var preprocessCount:Int;
    public var preprocessDelay:Int;
    public var cacheLimit:Int;
}