package afung.mangaWeb3.server.provider;

import afung.mangaWeb3.server.FileNotFoundException;
import afung.mangaWeb3.server.MangaContentMismatchException;
import afung.mangaWeb3.server.MangaWrongFormatException;
import afung.mangaWeb3.server.Settings;
import php.Exception;
import php.FileSystem;
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
                    content.push(fileName);
                }
            }
            
            rar.close();
            content.sort(function(a:String, b:String):Int
            {
                return untyped __call__("strnatcmp", a, b);
            });
        }
        
        return content;
    }
    
    public function OutputFile(path:String, content:String, outputPath:String):String
    {
        if (!FileSystem.exists(path))
        {
            throw new FileNotFoundException(path);
        }
        
        var rar:RarArchive = RarArchive.open(path);
        var result:Dynamic = rar;
        
        if (result != false && !rar.isBroken())
        {
            var entry:RarEntry = rar.getEntry(content);
            result = entry;
            outputPath = outputPath + "." + Path.extension(content).toLowerCase();
            
            if (result != false && !entry.isDirectory() && !entry.isEncrypted() && entry.extract("", outputPath))
            {
            }
            else
            {
                rar.close();
                throw new MangaContentMismatchException(path);
            }
            
            rar.close();
        }
        else
        {
            throw new MangaWrongFormatException(path);
        }
        
        return outputPath;
    }
}