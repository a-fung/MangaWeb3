package afung.mangaWeb3.server.provider;

import afung.mangaWeb3.server.FileNotFoundException;
import afung.mangaWeb3.server.MangaContentMismatchException;
import afung.mangaWeb3.server.MangaWrongFormatException;
import afung.mangaWeb3.server.Settings;
import afung.mangaWeb3.server.Utility;
import haxe.Utf8;
import php.Exception;
import php.FileSystem;
import php.io.Path;
import php.Lib;
import php.NativeArray;
import ZipArchive;

/**
 * ...
 * @author a-fung
 */

class ZipProvider implements IMangaProvider
{
    public static var Extension:String = ".zip";
    
    public function new()
    {
        if (!Settings.UseZip)
        {
            throw new Exception("Zip format is not configured to use.");
        }
    }
    
    public function TryOpen(path:String):Bool
    {
        var validFile:Bool = false;
        var zip:ZipArchive = new ZipArchive();
        var result:Dynamic = zip.open(path);
        
        if (result == true)
        {
            for (index in 0...zip.numFiles)
            {
                var fileName:String = zip.getNameIndex(index);
                var extension:String = "." + Path.extension(fileName).toLowerCase();
                
                if (Utility.ArrayContains(Constants.FileExtensionsInArchive, extension))
                {
                    validFile = true;
                    break;
                }
            }
            
            zip.close();
        }
        
        return validFile;
    }
    
    public function GetContent(path:String):Array<String>
    {
        var content:Array<String> = new Array<String>();
        var zip:ZipArchive = new ZipArchive();
        var result:Dynamic = zip.open(path);
        
        if (result == true)
        {
            for (index in 0...zip.numFiles)
            {
                var fileName:String = zip.getNameIndex(index);
                var extension:String = "." + Path.extension(fileName).toLowerCase();
                
                if (Utility.ArrayContains(Constants.FileExtensionsInArchive, extension))
                {
                    content.push(Utf8.encode(fileName));
                }
            }
            
            zip.close();
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
        
        var zip:ZipArchive = new ZipArchive();
        var result:Dynamic = zip.open(path);
        content = Utf8.decode(content);
        
        if (result == true)
        {
            outputPath = outputPath + "." + Path.extension(content).toLowerCase();
            
            if (zip.renameName(content, outputPath) && zip.extractTo("./", outputPath))
            {
            }
            else
            {
                zip.unchangeAll();
                zip.close();
                throw new MangaContentMismatchException(path);
            }
            
            zip.unchangeAll();
            zip.close();
        }
        else
        {
            throw new MangaWrongFormatException(path);
        }
        
        return outputPath;
    }
}