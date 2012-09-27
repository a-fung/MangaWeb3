package afung.mangaWeb3.server.provider;

import afung.mangaWeb3.server.Settings;
import php.Exception;
import php.io.Path;
import php.Lib;
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
        
        if (result == true && !rar.isBroken())
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
}