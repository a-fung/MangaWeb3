package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class MangaPageRequest extends JsonRequest
{
    public var id:Int;
    public var page:Int;
    public var width:Int;
    public var height:Int;
    public var part:Int;
    public var dimensions:Bool;
}