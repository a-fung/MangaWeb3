package afung.mangaWeb3.server.provider;

/**
 * ...
 * @author a-fung
 */

interface IMangaProvider 
{
    function TryOpen(path:String):Bool;
    
    function GetContent(path:String):Array<String>;
    
    function OutputFile(path:String, content:String, outputPath:String):String;
}