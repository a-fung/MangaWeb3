package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionsUsersAccessRequest extends JsonRequest
{
    public var t:Int;
    public var id:Int;
    public var ids:Array<Int>;
    public var access:Bool;
}