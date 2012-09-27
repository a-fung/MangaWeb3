package afung.mangaWeb3.server;

import afung.mangaWeb3.server.provider.PdfProvider;
import afung.mangaWeb3.server.provider.RarProvider;
import afung.mangaWeb3.server.provider.ZipProvider;
import php.FileSystem;
import php.io.Path;

/**
 * ...
 * @author a-fung
 */

class Manga 
{
    public var Id(default, null):Int;
    
    public var ParentCollection(default, null):Collection;
    
    public var MangaPath(default, null):String;
    
    public var MangaType(default, null):Int;
    
    public var Content(default, null):String;
    
    public var View(default, null):Int;
    
    public var Status(default, null):Int;
    
    private function new()
    {
        Id = -1;
    }
    
    public static function CheckMangaPath(path:String):String
    {
        path = FileSystem.fullPath(path);
        
        if (path == null || !FileSystem.exists(path) || FileSystem.isDirectory(path))
        {
            return null;
        }
        
        return path;
    }
    
    public static function CheckMangaType(path:String):Int
    {
        var extension:String = "." + Path.extension(path).toLowerCase();
        
        if (Settings.UseZip && extension == ZipProvider.Extension && new ZipProvider().TryOpen(path))
        {
            return 0;
        }
        
        if (Settings.UseRar && extension == RarProvider.Extension && new RarProvider().TryOpen(path))
        {
            return 1;
        }
        
        if (Settings.UsePdf && extension == PdfProvider.Extension && new PdfProvider().TryOpen(path))
        {
            return 2;
        }
        
        return -1;
    }
}