package afung.mangaWeb3.server;

import afung.mangaWeb3.common.AdminMangaMetaJson;
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
    
    private var _meta:MangaMeta = null;
    
    public var Meta(get_Meta, never):MangaMeta;
    
    private function get_Meta():MangaMeta
    {
        if (_meta == null && Id != -1)
        {
            _meta = MangaMeta.Get(this);
        }
        
        return _meta;
    }
    
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
        newManga._meta = MangaMeta.CreateNewMeta(newManga);
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
    
    public static function GetMangasWithFilter(collection:Collection, tag:String, author:String, type:Int):Array<Manga>
    {
        var where:String = "TRUE";

        if (collection != null)
        {
            where += " AND `cid`=" + Database.Quote(Std.string(collection.Id));
        }

        if (tag != null && tag != "")
        {
            where += " AND `id` IN (SELECT `mid` FROM `mangatag` WHERE `tid` IN (SELECT `id` FROM `tag` WHERE `name`=" + Database.Quote(tag) + "))";
        }

        if (author != null && author != "")
        {
            where += " AND `id` IN (SELECT `mid` FROM `meta` WHERE `author`=" + Database.Quote(author) + ")";
        }

        if (type != -1)
        {
            where += " AND `type`=" + Database.Quote(Std.string(type));
        }
        
        return GetMangas(where);
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
        obj.title = Meta.Title;
        obj.collection = ParentCollection.Name;
        obj.path = MangaPath;
        obj.type = MangaType;
        obj.view = View;
        obj.status = Status;
        return obj;
    }
    
    public function GetMetaJson():AdminMangaMetaJson
    {
        var obj:AdminMangaMetaJson = new AdminMangaMetaJson();
        obj.author = Meta.Author;
        obj.title = Meta.Title;
        obj.volume = Meta.Volume;
        obj.series = Meta.Series;
        obj.year = Meta.Year;
        obj.publisher = Meta.Publisher;
        obj.tags = GetTags();
        return obj;
    }
    
    public function UpdateMeta(obj:AdminMangaMetaJson):Void
    {
        Meta.Update(obj);
        UpdateTags(obj.tags);
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
        UpdateTags([]);
        Database.Delete("manga", "`id`=" + Database.Quote(Std.string(Id)));
        Database.Delete("meta", "`mid`=" + Database.Quote(Std.string(Id)));
    }
    
    public static function GetAllTags():Array<String>
    {
        return Database.GetDistinctStringValues("tag", "name");
    }
    
    private function GetTags():Array<String>
    {
        return Database.GetDistinctStringValues("tag", "name", "`id` IN (SELECT `tid` FROM `mangatag` WHERE `mid`=" + Database.Quote(Std.string(Id)) + ")");
    }
    
    private function UpdateTags(tags:Array<String>):Void
    {
        var id:Int;
        var oldTags:Array<String> = GetTags();
        var allTags:Array<String> = GetAllTags();

        for (rawTag in tags)
        {
            var tag:String = Utility.Remove4PlusBytesUtf8Chars(rawTag);

            if (Utility.ArrayStringContains(oldTags, tag))
            {
                if (Utility.ArrayStringContains(allTags, tag))
                {
                    id = Std.parseInt(Database.Select("tag", "`name`=" + Database.Quote(tag))[0].get("id"));
                }
                else
                {
                    var tagData:Hash<Dynamic> = new Hash<Dynamic>();
                    tagData.set("name", tag);
                    Database.Insert("tag", tagData);
                    id = Database.LastInsertId();
                }

                var mangaTagData:Hash<Dynamic> = new Hash<Dynamic>();
                mangaTagData.set("tid", id); // tag ID
                mangaTagData.set("mid", Id); // manga ID
                Database.Insert("mangatag", mangaTagData);
            }
        }

        for (tag in oldTags)
        {
            if (Utility.ArrayStringContains(tags, tag))
            {
                id = Std.parseInt(Database.Select("tag", "`name`=" + Database.Quote(tag))[0].get("id"));
                Database.Delete("mangatag", "`tid`=" + Database.Quote(Std.string(id)) + " AND `mid`=" + Database.Quote(Std.string(Id)));
                if (Database.Select("mangatag", "`tid`=" + Database.Quote(Std.string(id))).length == 0)
                {
                    Database.Delete("tag", "`id`=" + Database.Quote(Std.string(id)));
                }
            }
        }
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