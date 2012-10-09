package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class MangaListItemDetailsResponse extends JsonResponse
{
    public var author:String;
    public var series:String;
    public var volume:Int;
    public var year:Int;
    public var publisher:String;
    public var tags:Array<String>;
}