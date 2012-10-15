package afung.mangaWeb3.server;

import afung.mangaWeb3.common.ThreadHelperRequest;
import afung.mangaWeb3.server.provider.PdfProvider;
import afung.mangaWeb3.server.provider.RarProvider;
import afung.mangaWeb3.server.provider.ZipProvider;
import haxe.Json;
import php.Exception;
import php.FileSystem;
import php.Web;
import sys.FileStat;

/**
 * ...
 * @author a-fung
 */

class ThreadHelper 
{
    public static function Run(methodName:String, parameters:Array<Dynamic>):Void
    {
        var request:ThreadHelperRequest = new ThreadHelperRequest();
        request.methodName = methodName;
        request.parameters = parameters;
        
        var jsonRequestString = Json.stringify(request);
        
        var errno:Int = 0;
        var errstr:String = "";
        var port:Int = untyped __var__("_SERVER", "SERVER_PORT");
        var host:String = (port == 443 ? "ssl://" : "") + untyped __var__("_SERVER", "SERVER_ADDR");
        var sckt:Dynamic = untyped __call__("fsockopen", host, port, errno, errstr, 30);
        var post_string:String = "j=" + untyped __call__("urlencode", jsonRequestString);
        
        var out:String = "POST " + untyped __var__("_SERVER", "REQUEST_URI") + " HTTP/1.1\r\n";
        out += "Host: " + untyped __var__("_SERVER", "SERVER_NAME") + "\r\n";
        out += "Content-Type: application/x-www-form-urlencoded\r\n";
        out += "Content-Length: " + Std.string(post_string.length) + "\r\n";
        out += "Connection: Close\r\n\r\n";
        out += post_string;

        untyped __call__("fwrite", sckt, out);
        untyped __call__("fclose", sckt);
    }
    
    public static function InnerRun(methodName:String, parameters:Array<Dynamic>):Void
    {
        switch(methodName)
        {
            case "MangaProcessFile":
                MangaProcessFile(parameters);
            case "MangaCacheLimit":
                MangaCacheLimit(parameters);
            case "MangaPreprocessFiles":
                MangaPreprocessFiles(parameters);
            case "MangaPreprocessParts":
                MangaPreprocessParts(parameters);
            case "ProcessAutoAddStage1":
                ProcessAutoAddStage1(parameters);
            case "ProcessAutoAddStage2":
                ProcessAutoAddStage2(parameters);
            default:
                return;
        }
    }
    
    private static function MangaProcessFile(parameters:Array<Dynamic>):Void
    {
        var id:Int = parameters[0];
        var manga:Manga = Manga.GetById(id);
        
        if (manga != null)
        {
            manga.ProcessFile(parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6]);
        }
    }
    
    private static function MangaCacheLimit(parameters:Array<Dynamic>):Void
    {
        Manga.CacheLimit();
    }
    
    private static function MangaPreprocessFiles(parameters:Array<Dynamic>):Void
    {
        var id:Int = parameters[0];
        var manga:Manga = Manga.GetById(id);
        
        if (manga != null)
        {
            var page:Int = parameters[1];
            
            for (i in 1...6)
            {
                if (page + i >= 0 && page + i < manga.NumberOfPages)
                {
                    manga.GetPage(page + i, parameters[2], parameters[3], 0);
                }

                if (page - i >= 0 && page - i < manga.NumberOfPages)
                {
                    manga.GetPage(page - i, parameters[2], parameters[3], 0);
                }
            }
        }
    }
    
    private static function MangaPreprocessParts(parameters:Array<Dynamic>):Void
    {
        var id:Int = parameters[0];
        var manga:Manga = Manga.GetById(id);
        
        if (manga != null)
        {
            manga.GetPage(parameters[1], parameters[2], parameters[3], 1);
            manga.GetPage(parameters[1], parameters[2], parameters[3], 2);
        }
    }
    
    private static function ProcessAutoAddStage1(parameters:Array<Dynamic>):Void
    {
        if (Date.now().getTime() / 1000 - Settings.LastAutoAddProcessTime < 300)
        {
            return;
        }
        
        Settings.LastAutoAddProcessTime = Math.round(Date.now().getTime() / 1000);
        var filesUnderCollections:Array<Array<Dynamic>> = [];
        var collections:Array<Collection> = Collection.GetAutoAdd();
        var directoriesToRead:Array<Array<Dynamic>> = [];
        
        for (collection in collections)
        {
            directoriesToRead.push([collection.Path, collection.Id]);
        }
        
        ThreadHelper.Run("ProcessAutoAddStage2", [directoriesToRead, null, []]);
    }
    
    private static function ProcessAutoAddStage2(parameters:Array<Dynamic>):Void
    {
        untyped __call__("set_time_limit", 60);
        Settings.LastAutoAddProcessTime = Math.round(Date.now().getTime() / 1000);
        var directoriesToRead:Array<Array<Dynamic>> = parameters[0];
        var currentDirectory:Array<Dynamic> = parameters[1];
        var filesInCurrentDirectory:Array<String> = parameters[2];
        var directoryPath:String = null;
        
        if (filesInCurrentDirectory.length != 0)
        {
            directoryPath = currentDirectory[0];
            var file:String = filesInCurrentDirectory[Math.floor(Math.random() * filesInCurrentDirectory.length)];
            filesInCurrentDirectory.remove(file);
            
            if (Utility.IsValidStringForDatabase(file))
            {
                if (FileSystem.isDirectory(directoryPath + file))
                {
                    directoriesToRead.push([directoryPath + file + "/", currentDirectory[1]]);
                }
                else if ((file.toLowerCase().substr(file.length - ZipProvider.Extension.length) == ZipProvider.Extension && Settings.UseZip) ||
                    (file.toLowerCase().substr(file.length - RarProvider.Extension.length) == RarProvider.Extension && Settings.UseRar) ||
                    (file.toLowerCase().substr(file.length - PdfProvider.Extension.length) == PdfProvider.Extension && Settings.UsePdf))
                {
                    try
                    {
                        // try to add file here
                        var collection:Collection;
                        var path:String = directoryPath + file;
                        
                        if ((path = Manga.CheckMangaPath(path)) != null && (collection = Collection.GetById(currentDirectory[1])) != null)
                        {
                            if (path.indexOf(collection.Path) == 0 && Manga.CheckMangaType(path) != -1)
                            {
                                Manga.CreateNewManga(collection, path).Save();
                            }
                        }
                    }
                    catch (e:Exception)
                    {
                    }
                }
            }
            
            ThreadHelper.Run("ProcessAutoAddStage2", [directoriesToRead, currentDirectory, filesInCurrentDirectory]);
        }
        else if (directoriesToRead.length != 0)
        {
            currentDirectory = directoriesToRead[Math.floor(Math.random() * directoriesToRead.length)];
            directoriesToRead.remove(currentDirectory);
            directoryPath = currentDirectory[0];
            
            filesInCurrentDirectory = FileSystem.readDirectory(directoryPath);
            
            ThreadHelper.Run("ProcessAutoAddStage2", [directoriesToRead, currentDirectory, filesInCurrentDirectory]);
        }
    }
}