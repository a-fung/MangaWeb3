package afung.mangaWeb3.server.provider;

import afung.mangaWeb3.server.Settings;
import php.Exception;

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
}