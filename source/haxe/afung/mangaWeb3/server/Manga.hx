package afung.mangaWeb3.server;

import afung.mangaWeb3.common.MangaJson;
import afung.mangaWeb3.server.provider.IMangaProvider;
import afung.mangaWeb3.server.provider.PdfProvider;
import afung.mangaWeb3.server.provider.RarProvider;
import afung.mangaWeb3.server.provider.ZipProvider;
import php.Exception;
import php.FileSystem;
import php.io.Path;
import php.Lib;
import sys.FileStat;

/**
 * ...
 * @author a-fung
 */

class Manga 
{
    public var Id(default, null):Int;
    
    public var ParentCollection(default, null):Collection;
    
    public var MangaPath(default, null):String;
    
    public var MangaType(default, null):Int;
    
    public var Content(default, null):Array<String>;
    
    public var ModifiedTime(default, null):Int;
    
    public var Size(default, null):Int;
    
    public var NumberOfPages(default, null):Int;
    
    public var View(default, null):Int;
    
    public var Status(default, null):Int;
    
    private var provider:IMangaProvider;
    
    private var Provider(get_Provider, never):IMangaProvider;
    
    private function get_Provider():IMangaProvider
    {
        if (provider == null)
        {
            switch(MangaType)
            {
                case 0:
                    provider = new ZipProvider();
                case 1:
                    provider = new RarProvider();
                case 2:
                    provider = new PdfProvider();
                default:
                    throw new Exception("Invalid MangaType");
            }
        }
        
        return provider;
    }
    
    public var Meta(default, null):MangaMeta;
    
    private function new()
    {
        Id = -1;
    }
    
    public static function CreateNewManga(collection:Collection, path:String):Manga
    {
        var newManga:Manga = new Manga();
        newManga.ParentCollection = collection;
        newManga.MangaPath = path;
        newManga.MangaType = CheckMangaType(path);
        newManga.InnerRefreshContent();
        newManga.View = newManga.Status = 0;
        newManga.Meta = MangaMeta.CreateNewMeta(newManga);
        return newManga;
    }
    
    private static function FromData(data:Hash<Dynamic>):Manga
    {
        var newManga:Manga = new Manga();
        newManga.Id = Std.parseInt(data.get("id"));
        newManga.ParentCollection = Collection.GetById(Std.parseInt(data.get("cid")));
        newManga.MangaPath = Std.string(data.get("path"));
        newManga.MangaType = Std.parseInt(data.get("type"));
        newManga.ModifiedTime = Std.parseInt(data.get("time"));
        newManga.Size = Std.parseInt(data.get("size"));
        newManga.Content = cast Lib.toHaxeArray(untyped __call__("json_decode", Std.string(data.get("content"))));
        newManga.NumberOfPages = Std.parseInt(data.get("numpages"));
        newManga.View = Std.parseInt(data.get("view"));
        newManga.Status = Std.parseInt(data.get("status"));
        return newManga;
    }
    
    public static function GetById(id:Int):Manga
    {
        var resultSet:Array<Hash<Dynamic>> = Database.Select("manga", "`id`=" + Database.Quote(Std.string(id)));
        
        if (resultSet.length > 0)
        {
            return FromData(resultSet[0]);
        }
        
        return null;
    }
    
    public static function GetByPath(path:String):Manga
    {
        if (path != null && path != "")
        {
            var resultSet:Array<Hash<Dynamic>> = Database.Select("manga", "`path` COLLATE utf8_bin =" + Database.Quote(path));
            
            if (resultSet.length > 0)
            {
                return FromData(resultSet[0]);
            }
        }
        
        return null;
    }
    
    private static function GetMangas(where:String):Array<Manga>
    {
        var resultSet:Array<Hash<Dynamic>> = Database.Select("manga", where);
        var mangas:Array<Manga> = new Array<Manga>();
        
        for (result in resultSet)
        {
            mangas.push(FromData(result));
        }
        
        return mangas;
    }
    
    public static function GetAllMangas():Array<Manga>
    {
        return GetMangas(null);
    }
    
    public static function CheckMangaPath(path:String):String
    {
        path = FileSystem.fullPath(path);
        
        if (path == null || !FileSystem.exists(path) || FileSystem.isDirectory(path) || GetByPath(path) != null)
        {
            return null;
        }
        
        return path;
    }
    
    public static function CheckMangaType(path:String):Int
    {
        var extension:String = "." + Path.extension(path).toLowerCase();
        
        if (Settings.UseZip && extension == ZipProvider.Extension && new ZipProvider().TryOpen(path))
        {
            return 0;
        }
        
        if (Settings.UseRar && extension == RarProvider.Extension && new RarProvider().TryOpen(path))
        {
            return 1;
        }
        
        if (Settings.UsePdf && extension == PdfProvider.Extension && new PdfProvider().TryOpen(path))
        {
            return 2;
        }
        
        return -1;
    }
    
    public function RefreshContent():Void
    {
        if (!FileSystem.exists(MangaPath) || FileSystem.isDirectory(MangaPath))
        {
            Status = 1;
        }
        else
        {
            InnerRefreshContent();
            
            if (Content.length == 0)
            {
                Status = 2;
            }
            else
            {
                Status = 0;
            }
        }
        
        Save();
    }
    
    private function InnerRefreshContent():Void
    {
        var info:FileStat = FileSystem.stat(MangaPath);
        ModifiedTime = Math.round(info.mtime.getTime());
        Size = info.size;
        Content = Provider.GetContent(MangaPath);
        NumberOfPages = Content.length;
    }
    
    public function ChangePath(newPath:String, newType:Int):Void
    {
        MangaPath = newPath;
        MangaType = newType;
        InnerRefreshContent();
        Status = 0;
        Save();
    }
    
    public function Save():Void
    {
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("cid", ParentCollection.Id);
        data.set("path", MangaPath);
        data.set("type", MangaType);
        data.set("content", untyped __call__("json_encode", Lib.toPhpArray(Content)));
        data.set("time", ModifiedTime);
        data.set("size", Size);
        data.set("numpages", NumberOfPages);
        data.set("view", View);
        data.set("status", Status);

        if (Id == -1)
        {
            Database.Insert("manga", data);
            Id = Database.LastInsertId();
            Meta.Save();
        }
        else
        {
            data.set("id", Id);
            Database.Replace("manga", data);
        }
    }
    
    public function ToJson():MangaJson
    {
        var obj:MangaJson = new MangaJson();
        obj.id = Id;
        obj.collection = ParentCollection.Name;
        obj.path = MangaPath;
        obj.type = MangaType;
        obj.view = View;
        obj.status = Status;
        return obj;
    }
    
    public static function ToJsonArray(mangas:Array<Manga>):Array<MangaJson>
    {
        var objs:Array<MangaJson> = new Array<MangaJson>();
        for (manga in mangas)
        {
            objs.push(manga.ToJson());
        }
        
        return objs;
    }
    
    public function Delete():Void
    {
        Database.Delete("manga", "`id`=" + Database.Quote(Std.string(Id)));
        Database.Delete("meta", "`mid`=" + Database.Quote(Std.string(Id)));

        // TODO: delete tags
    }
    
    public static function DeleteMangas(mangas:Array<Manga>):Void
    {
        for (manga in mangas)
        {
            manga.Delete();
        }
    }
    
    public static function DeleteMangasFromCollectionIds(cids:Array<Int>):Void
    {
        DeleteMangas(GetMangas(Database.BuildWhereClauseOr("cid", cids)));
    }
    
    public static function DeleteMangasFromIds(ids:Array<Int>):Void
    {
        DeleteMangas(GetMangas(Database.BuildWhereClauseOr("id", ids)));
    }
    
    public static function RefreshMangasContent(ids:Array<Int>):Void
    {
        for (manga in GetMangas(Database.BuildWhereClauseOr("id", ids)))
        {
            manga.RefreshContent();
        }
    }
}