package afung.mangaWeb3.server.provider;

import php.io.File;
import php.io.Path;
import php.Lib;

/**
 * ...
 * @author a-fung
 */

class ImageProvider 
{
    public static function ResizeFile(inputFile:String, outputFile:String, width:Int, height:Int):Void
    {
        var image:Dynamic;
        var ext:String = "." + Path.extension(inputFile).toLowerCase();
        switch(ext)
        {
            case ".gif":
                image = untyped __call__("imagecreatefromgif", inputFile);
            case ".jpg", ".jpeg":
                image = untyped __call__("imagecreatefromjpeg", inputFile);
            case ".png":
                image = untyped __call__("imagecreatefrompng", inputFile);
            default:
                return;
        }
        
        if (image != false)
        {
            var imageSize:Array<Dynamic> = Lib.toHaxeArray(untyped __call__("getimagesize", inputFile));
            var resizedImage:Dynamic = untyped __call__("imagecreatetruecolor", width, height);
            untyped __call__("imagecopyresampled", resizedImage, image, 0, 0, 0, 0, width, height, imageSize[0], imageSize[1]);
            untyped __call__("imagejpeg", resizedImage, outputFile, 90);
            untyped __call__("imagedestroy", resizedImage);
            untyped __call__("imagedestroy", image);
        }
    }
    
    public static function GetDimensions(inputFile:String):Array<Int>
    {
        var image:Dynamic = untyped __call__("getimagesize", inputFile);
        
        if (image != false)
        {
            var imageSize:Array<Dynamic> = Lib.toHaxeArray(image);
            return [imageSize[0], imageSize[1]];
        }
        
        return null;
    }
}