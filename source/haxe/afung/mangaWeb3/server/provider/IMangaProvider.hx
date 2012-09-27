package afung.mangaWeb3.server.provider;

/**
 * ...
 * @author a-fung
 */

interface IMangaProvider 
{
    function TryOpen(path:String):Bool;
}