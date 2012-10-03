package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class AdminMangaMetaGetResponse extends JsonResponse
{
    public var meta:AdminMangaMetaJson;
    public var authors:Array<String>;
    public var series:Array<String>;
    public var publishers:Array<String>;
}