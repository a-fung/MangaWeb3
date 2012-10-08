package afung.mangaWeb3.server.provider;

import afung.mangaWeb3.server.Settings;
import php.Exception;
import php.FileSystem;

/**
 * ...
 * @author a-fung
 */

class PdfProvider implements IMangaProvider
{
    public static var Extension:String = ".pdf";
    
    public function new()
    {
        if (!Settings.UsePdf)
        {
            throw new Exception("PDF format is not configured to use.");
        }
    }
    
    private function GetNumberOfPages(path:String):Int
    {
        Native.Exec("pdfinfo \"" + path + "\"");
        var output:String = Utility.JoinNativeArray(Native.ExecOutput);
        var exitCode:Int = Native.ExecReturnVar;
        
        if (exitCode == 0)
        {
            var index:Int = output.indexOf("Pages:");
            if (index != -1)
            {
                var index2:Int = output.indexOf("\n", index);
                return Std.parseInt(output.substr(index + 6, index2 - index - 6));
            }
        }
        
        return 0;
    }
    
    public function TryOpen(path:String):Bool
    {
        return GetNumberOfPages(path) > 0;
    }
    
    public function GetContent(path:String):Array<String>
    {
        var pages:Int = GetNumberOfPages(path);
        var content:Array<String> = new Array<String>();
        for (p in 1...(pages + 1))
        {
            content.push(Std.string(p));
        }
        
        return content;
    }
    
    public function OutputFile(path:String, page:String, outputPath:String):String
    {
        var pageInt:Int = Std.parseInt(page);
        var numberOfPages:Int;
        if (pageInt < 1 || pageInt > (numberOfPages = GetNumberOfPages(path)))
        {
            throw new Exception("Read PDF file error", FileSystem.exists(path) ? (numberOfPages == 0 ? 1002 : 1003) : 1001);
        }
        
        outputPath = outputPath + ".png";
        Native.Exec("pdfdraw -o \"" + outputPath + "\" -r 300 \"" + path + "\" " + page);
        var exitCode:Int = Native.ExecReturnVar;
        
        if (exitCode != 0)
        {
            throw new Exception("Read PDF file error", 1002);
        }
        
        return outputPath;
    }
}