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
        var files:Array<Array<Dynamic>> = new Array<Array<Dynamic>>();
        var collections:Array<Collection> = Collection.GetAutoAdd();
        
        for (collection in collections)
        {
            var resultSet:Array<Hash<Dynamic>> = Database.Select("manga", "`cid`=" + Database.Quote(Std.string(collection.Id)), null, null, "`path`");
            var filesUnderCollection:Array<Array<Dynamic>> = new Array<Array<Dynamic>>();
            ReadDirectory(collection.Path, filesUnderCollection);
            
            for (fileUnderCollection in filesUnderCollection)
            {
                var fileStat:FileStat = fileUnderCollection[1];
                if (Settings.LastAutoAddProcessTime - fileStat.mtime.getTime() / 1000 <= 60)
                {
                    continue;
                }
                
                var hit:Bool = false;
                for (result in resultSet)
                {
                    if (fileUnderCollection[0] == Std.string(result.get("path")))
                    {
                        hit = true;
                        break;
                    }
                }
                
                if (!hit)
                {
                    files.push([fileUnderCollection[0], collection.Id]);
                }
            }
        }
        
        ThreadHelper.Run("ProcessAutoAddStage2", [files, 0]);
    }
    
    private static function ReadDirectory(directoryPath:String, filesUnderCollection:Array<Array<Dynamic>>):Void
    {
        for (file in FileSystem.readDirectory(directoryPath))
        {
            if (FileSystem.isDirectory(directoryPath + file))
            {
                ReadDirectory(directoryPath + file + "/", filesUnderCollection);
            }
            else if ((Settings.UseZip && file.toLowerCase().indexOf(ZipProvider.Extension) == file.length - ZipProvider.Extension.length) ||
                (Settings.UseRar && file.toLowerCase().indexOf(RarProvider.Extension) == file.length - RarProvider.Extension.length) ||
                (Settings.UsePdf && file.toLowerCase().indexOf(PdfProvider.Extension) == file.length - PdfProvider.Extension.length))
            {
                filesUnderCollection.push([directoryPath + file, FileSystem.stat(directoryPath + file)]);
            }
        }
    }
    
    private static function ProcessAutoAddStage2(parameters:Array<Dynamic>):Void
    {
        Settings.LastAutoAddProcessTime = Math.round(Date.now().getTime() / 1000);
        var files:Array<Array<Dynamic>> = parameters[0];
        var index:Int = parameters[1];
        
        if (index >= files.length)
        {
            return;
        }
        
        try
        {
            var collection:Collection = Collection.GetById(files[index][1]);
            var path:String = files[index][0];
            
            if (collection != null && (path = Manga.CheckMangaPath(path)) != null && Utility.IsValidStringForDatabase(path))
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
        
        ThreadHelper.Run("ProcessAutoAddStage2", [files, index + 1]);
    }
}