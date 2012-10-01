package afung.mangaWeb3.server.provider;

import afung.mangaWeb3.server.Settings;
import afung.mangaWeb3.server.Utility;
import haxe.Utf8;
import php.Exception;
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
            var nContent:NativeArray = Lib.toPhpArray(content);
            untyped __call__("natsort", nContent);
            content = cast Lib.toHaxeArray(nContent);
        }
        
        return content;
    }
}