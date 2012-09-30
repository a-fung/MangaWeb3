package afung.mangaWeb3.server.provider;

import afung.mangaWeb3.server.Settings;
import php.Exception;
import php.io.Path;
import php.Lib;
import php.NativeArray;
import RarArchive;
import RarEntry;
import RarException;

/**
 * ...
 * @author a-fung
 */

class RarProvider implements IMangaProvider
{
    public static var Extension:String = ".rar";
    
    public function new()
    {
        if (!Settings.UseRar)
        {
            throw new Exception("RAR format is not configured to use.");
        }
    }
    
    public function TryOpen(path:String):Bool
    {
        var validFile:Bool = false;
        var rar:RarArchive = RarArchive.open(path);
        var result:Dynamic = rar;
        
        if (result != false && !rar.isBroken())
        {
            var entries:Array<RarEntry> = cast Lib.toHaxeArray(rar.getEntries());
            
            for (entry in entries)
            {
                if (entry.isDirectory() || entry.isEncrypted())
                {
                    continue;
                }
                
                var extension:String = "." + Path.extension(entry.getName()).toLowerCase();
                
                if (Utility.ArrayContains(Constants.FileExtensionsInArchive, extension))
                {
                    validFile = true;
                    break;
                }
            }
            
            rar.close();
        }
        
        return validFile;
    }
    
    public function GetContent(path:String):Array<String>
    {
        var content:Array<String> = new Array<String>();
        var rar:RarArchive = RarArchive.open(path);
        var result:Dynamic = rar;
        
        if (result != false && !rar.isBroken())
        {
            var entries:Array<RarEntry> = cast Lib.toHaxeArray(rar.getEntries());
            
            for (entry in entries)
            {
                if (entry.isDirectory() || entry.isEncrypted())
                {
                    continue;
                }
                
                var fileName:String = entry.getName();
                var extension:String = "." + Path.extension(fileName).toLowerCase();
                
                if (Utility.ArrayContains(Constants.FileExtensionsInArchive, extension))
                {
                    content.push(Utility.Remove4PlusBytesUtf8Chars(fileName));
                }
            }
            
            rar.close();
            var nContent:NativeArray = Lib.toPhpArray(content);
            untyped __call__("natsort", nContent);
            content = cast Lib.toHaxeArray(nContent);
        }
        
        return content;
    }
}