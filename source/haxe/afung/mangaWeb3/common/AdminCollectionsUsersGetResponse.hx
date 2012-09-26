package afung.mangaWeb3.common;

/**
 * ...
 * @author a-fung
 */

class AdminCollectionsUsersGetResponse extends JsonResponse
{
    public var name:String;
    public var data:Array<CollectionUserJson>;
    public var names:Array<String>;
}