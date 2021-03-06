package afung.mangaWeb3.server;

import afung.mangaWeb3.common.AdminMangaMetaJson;
import afung.mangaWeb3.common.MangaFilter;
import afung.mangaWeb3.common.MangaJson;
import afung.mangaWeb3.common.MangaListItemJson;
import afung.mangaWeb3.server.provider.ImageProvider;
import afung.mangaWeb3.server.provider.IMangaProvider;
import afung.mangaWeb3.server.provider.PdfProvider;
import afung.mangaWeb3.server.provider.RarProvider;
import afung.mangaWeb3.server.provider.ZipProvider;
import haxe.Json;
import haxe.Utf8;
import php.Exception;
import php.FileSystem;
import php.io.File;
import php.io.FileOutput;
import php.io.Path;
import php.Lib;
import php.Sys;
import sys.FileStat;

/**
 * ...
 * @author a-fung
 */

class Manga 
{
    public var Id(default, null):Int;
    
    public var ParentCollectionId(default, null):Int;
    
    private var _parentCollection:Collection;
    
    public var ParentCollection(get_ParentCollection, never):Collection;
    
    private function get_ParentCollection():Collection
    {
        if (_parentCollection == null)
        {
            _parentCollection = Collection.GetById(ParentCollectionId);
        }
        
        return _parentCollection;
    }
    
    public var Title(default, null):String;
    
    public var MangaPath(default, null):String;
    
    public var MangaType(default, null):Int;
    
    private var _content:Array<String>;
    
    public var Content(get_Content, set_Content):Array<String>;
    
    private function get_Content():Array<String>
    {
        if (_content == null && Id != -1)
        {
            _content = cast Lib.toHaxeArray(untyped __call__("json_decode", Std.string(Database.Select("mangacontent", "`id`='" + Id + "'", null, null, "`content`")[0].get("content"))));
        }
        
        return _content;
    }
    
    private function set_Content(value:Array<String>):Array<String>
    {
        return _content = value;
    }
    
    private var _dimensions:Array<Array<Int>>;
    
    private var Dimensions(get_Dimensions, set_Dimensions):Array<Array<Int>>;
    
    private function get_Dimensions():Array<Array<Int>>
    {
        if (_dimensions == null && Id != -1)
        {
            _dimensions = Json.parse(Std.string(Database.Select("mangadimensions", "`id`='" + Id + "'", null, null, "`dimensions`")[0].get("dimensions")));
        }
        
        return _dimensions;
    }
    
    private function set_Dimensions(value:Array<Array<Int>>):Array<Array<Int>>
    {
        return _dimensions = value;
    }
    
    public var ModifiedTime(default, null):Int;
    
    public var Size(default, null):Int;
    
    public var NumberOfPages(default, null):Int;
    
    public var View(default, null):Int;
    
    public var Status(default, null):Int;
    
    public var LeftToRight(default, default):Bool;
    
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
        newManga.ParentCollectionId = collection.Id;
        newManga._parentCollection = collection;
        newManga.MangaPath = path;
        newManga.MangaType = CheckMangaType(path);
        newManga.InnerRefreshContent();
        newManga.View = newManga.Status = 0;
        newManga._meta = MangaMeta.CreateNewMeta(newManga);
        newManga.LeftToRight = false;

        var title:String = newManga.MangaPath.substr(0, newManga.MangaPath.lastIndexOf("."));
        title = title.substr(title.lastIndexOf("/") + 1);
        newManga.Title = Utf8.length(title) > 100 ? Utf8.sub(title, 0, 100) : title;
        return newManga;
    }
    
    private static function FromData(data:Hash<Dynamic>):Manga
    {
        var newManga:Manga = new Manga();
        newManga.Id = Std.parseInt(data.get("id"));
        newManga.ParentCollectionId = Std.parseInt(data.get("cid"));
        newManga.Title = Std.string(data.get("title"));
        newManga.MangaPath = Std.string(data.get("path"));
        newManga.MangaType = Std.parseInt(data.get("type"));
        newManga.ModifiedTime = Std.parseInt(data.get("time"));
        newManga.Size = Std.parseInt(data.get("size"));
        newManga.NumberOfPages = Std.parseInt(data.get("numpages"));
        newManga.View = Std.parseInt(data.get("view"));
        newManga.Status = Std.parseInt(data.get("status"));
        newManga.LeftToRight = Std.parseInt(data.get("ltr")) == 1;
        return newManga;
    }
    
    public static function GetById(id:Int):Manga
    {
        var resultSet:Array<Hash<Dynamic>> = Database.Select("manga", "`id` IN (SELECT `mid` FROM `meta`) AND `id`=" + Database.Quote(Std.string(id)));
        
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
            var resultSet:Array<Hash<Dynamic>> = Database.Select("manga", "`id` IN (SELECT `mid` FROM `meta`) AND `path` COLLATE utf8_bin =" + Database.Quote(path));
            
            if (resultSet.length > 0)
            {
                return FromData(resultSet[0]);
            }
        }
        
        return null;
    }
    
    private static function GetSameFileName(cid:Int, path:String):Array<Manga>
    {
        var fileName:String = Database.Quote(path.substr(path.lastIndexOf("/")));
        fileName = Database.Quote(fileName.substr(1, fileName.length - 2));
        fileName = StringTools.replace(fileName.substr(1, fileName.length - 2), "%", "\\%");
        var where:String = "`cid`=" + Database.Quote(Std.string(cid)) + " AND `path` COLLATE utf8_bin LIKE '%" + fileName + "'";
        
        return GetMangas(where);
    }
    
    private static function GetMangas(where:String):Array<Manga>
    {
        var additionalWhere:String = "`id` IN (SELECT `mid` FROM `meta`)";
        var resultSet:Array<Hash<Dynamic>> = Database.Select("manga", where == null ? additionalWhere : where + " AND " + additionalWhere);
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
    
    public static function GetMangaList(ajax:AjaxBase, filter:MangaFilter):Array<Manga>
    {
        var where:String = "`status`='0'";
        var user:User = User.GetCurrentUser(ajax);
        var collectionSelect:String = "FALSE";
        if (Settings.AllowGuest || user != null)
        {
            collectionSelect += " OR `cid` IN (SELECT `id` FROM `collection` WHERE `public`='1')";
        }

        if (user != null)
        {
            collectionSelect += " OR `cid` IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(Std.string(user.Id)) + " AND `access`='1')";
            where += " AND `cid` NOT IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(Std.string(user.Id)) + " AND `access`='0')";
        }

        where += " AND (" + collectionSelect + ")";

        if (filter != null)
        {
            if (filter.tag != null && filter.tag != "")
            {
                where += " AND `id` IN (SELECT `mid` FROM `mangatag` WHERE `tid` IN (SELECT `id` FROM `tag` WHERE `name`=" + Database.Quote(filter.tag) + "))";
            }
            
            var folder:String = null;
            var folderSetting:Int = 2;

            if (filter.search != null)
            {
                var metaSelect:String = "TRUE";

                if (filter.search.title != null && filter.search.title != "")
                {
                    var title:String = Database.Quote(filter.search.title);
                    title = Database.Quote(title.substr(1, title.length - 2));
                    title = StringTools.replace(title.substr(1, title.length - 2), "%", "\\%");
                    where += " AND `title` LIKE '%" + title + "%'";
                }

                if (filter.search.author != null && filter.search.author != "")
                {
                    metaSelect += " AND `author`=" + Database.Quote(filter.search.author);
                }

                if (filter.search.series != null && filter.search.series != "")
                {
                    metaSelect += " AND `series`=" + Database.Quote(filter.search.series);
                }

                if (filter.search.year >= 0)
                {
                    metaSelect += " AND `year`=" + Database.Quote(Std.string(filter.search.year));
                }

                if (filter.search.publisher != null && filter.search.publisher != "")
                {
                    metaSelect += " AND `publisher`=" + Database.Quote(filter.search.publisher);
                }

                if (metaSelect != "TRUE")
                {
                    where += " AND `id` IN (SELECT `mid` FROM `meta` WHERE " + metaSelect + ")";
                }

                if (filter.search.folderSetting > 0 && filter.search.folder != null && filter.search.folder != "")
                {
                    folder = filter.search.folder;
                    folderSetting = filter.search.folderSetting;
                }
            }

            if (folder == null && filter.folder != null && filter.folder != "")
            {
                folder = filter.folder;
            }
            
            if (folderSetting > 0 && folder != null && folder != "")
            {
                var index:Int;
                var collectionName:String = (index = folder.indexOf("/")) == -1 ? folder : folder.substr(0, index);
                var relativePath = folder.substr(index + 1);
                var collection:Collection = Collection.GetByName(collectionName);

                if (collection == null)
                {
                    where += " AND FALSE";
                }
                else
                {
                    var actualPath:String = Database.Quote(index == -1 ? collection.Path.substr(0, collection.Path.length - 1) : collection.Path + relativePath);
                    actualPath = Database.Quote(actualPath.substr(1, actualPath.length - 2));
                    actualPath = StringTools.replace(actualPath.substr(1, actualPath.length - 2), "%", "\\%");
                    where += " AND `cid`=" + Database.Quote(Std.string(collection.Id));
                    where += " AND `path` COLLATE utf8_bin LIKE '" + actualPath + "/%'" + (folderSetting == 2 ? " AND `path` COLLATE utf8_bin NOT LIKE '" + actualPath + "/%/%'" : "");
                }
            }
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
        ModifiedTime = Math.round(info.mtime.getTime() / 1000);
        Size = info.size;
        Content = Provider.GetContent(MangaPath);
        Dimensions = new Array<Array<Int>>();
        for (i in 0...Content.length)
        {
            Dimensions.push(null);    
        }
        
        NumberOfPages = Content.length;
        DeleteCache();
    }
    
    public function ChangePath(newPath:String, newType:Int):Void
    {
        DeleteCache();
        MangaPath = newPath;
        MangaType = newType;
        InnerRefreshContent();
        Status = 0;
        Save();
        ParentCollection.MarkFolderCacheDirty();
    }
    
    public function Save():Void
    {
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("cid", ParentCollectionId);
        data.set("title", Title);
        data.set("path", MangaPath);
        data.set("type", MangaType);
        data.set("time", ModifiedTime);
        data.set("size", Size);
        data.set("numpages", NumberOfPages);
        data.set("view", View);
        data.set("status", Status);
        data.set("ltr", LeftToRight ? 1 : 0);

        if (Id == -1)
        {
            Id = Database.InsertAndReturnId("manga", data);
            Meta.Save();
            
            for (manga in Manga.GetSameFileName(ParentCollectionId, MangaPath))
            {
                if (manga.Id != Id && manga.ParentCollectionId == ParentCollectionId && manga.Size == Size && manga.NumberOfPages == manga.NumberOfPages && manga.IsFileMissing())
                {
                    View = manga.View;
                    Title = manga.Title;
                    Meta.Copy(manga.Meta);
                    UpdateTags(manga.GetTags());
                    Save();
                    manga.Delete();
                    break;
                }
            }
        }
        else
        {
            data.set("id", Id);
            Database.Replace("manga", data);
        }
        
        if (_content != null)
        {
            data = new Hash<Dynamic>();
            data.set("id", Id);
            data.set("content", untyped __call__("json_encode", Lib.toPhpArray(Content)));
            Database.Replace("mangacontent", data);
        }
        
        if (_dimensions != null)
        {
            data = new Hash<Dynamic>();
            data.set("id", Id);
            data.set("dimensions", Json.stringify(_dimensions));
            Database.Replace("mangadimensions", data);
        }
    }
    
    public function IsFileMissing():Bool
    {
        return !FileSystem.exists(MangaPath);
    }

    public function ToMangaListItemJson():MangaListItemJson
    {
        var obj:MangaListItemJson = new MangaListItemJson();
        obj.id = Id;
        obj.title = Title;
        obj.pages = NumberOfPages;
        obj.size = Size;
        obj.date = ModifiedTime;
        return obj;
    }
    
    public function ToJson():MangaJson
    {
        var obj:MangaJson = new MangaJson();
        obj.id = Id;
        obj.title = Title;
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
        obj.title = Title;
        obj.volume = Meta.Volume;
        obj.series = Meta.Series;
        obj.year = Meta.Year;
        obj.publisher = Meta.Publisher;
        obj.tags = GetTags();
        return obj;
    }
    
    public function UpdateMeta(obj:AdminMangaMetaJson):Void
    {
        Title = Utility.Remove4PlusBytesUtf8Chars(obj.title);
        Meta.Update(obj);
        UpdateTags(obj.tags);
        Save();
    }
    
    public static function ToListItemJsonArray(mangas:Array<Manga>):Array<MangaListItemJson>
    {
        var objs:Array<MangaListItemJson> = new Array<MangaListItemJson>();
        for (manga in mangas)
        {
            objs.push(manga.ToMangaListItemJson());
        }
        
        return objs;
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
        ParentCollection.MarkFolderCacheDirty();
    }
    
    public static function GetAllTags():Array<String>
    {
        return Database.GetDistinctStringValues("tag", "name");
    }
    
    public function GetTags():Array<String>
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
            if (Utf8.length(tag) > 100)
            {
                tag = Utf8.sub(tag, 0, 100);
            }

            if (!Utility.ArrayStringContains(oldTags, tag))
            {
                if (Utility.ArrayStringContains(allTags, tag))
                {
                    id = Std.parseInt(Database.Select("tag", "`name`=" + Database.Quote(tag))[0].get("id"));
                }
                else
                {
                    var tagData:Hash<Dynamic> = new Hash<Dynamic>();
                    tagData.set("name", tag);
                    id = Database.InsertAndReturnId("tag", tagData);
                }

                var mangaTagData:Hash<Dynamic> = new Hash<Dynamic>();
                mangaTagData.set("tid", id); // tag ID
                mangaTagData.set("mid", Id); // manga ID
                Database.Insert("mangatag", mangaTagData);
            }
        }

        for (tag in oldTags)
        {
            if (!Utility.ArrayStringContains(tags, tag))
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
    
    public function GetCover():String
    {
        var hash:String = Utility.Md5(MangaPath);
        var lockPath:String = "cover/" + hash + ".lock";
        var coverRelativePath:String = "cover/" + hash + ".jpg";
        var coverPath:String = coverRelativePath;

        if (CheckLockFile(lockPath))
        {
            return null;
        }
        else if (!FileSystem.exists(coverPath))
        {
            var resizedDimensions:Array<Int> = GetResizedDimensions(0, 260, 200);
            if (resizedDimensions != null)
            {
                ThreadHelper.Run("MangaProcessFile", [Id, Content[0], resizedDimensions[0], resizedDimensions[1], 0, coverPath, lockPath]);
            }

            return null;
        }
        else
        {
            return coverRelativePath;
        }
    }
    
    public function GetPage(page:Int, width:Int, height:Int, part:Int):String
    {
        var resizedDimensions:Array<Int> = GetResizedDimensions(page, width, height);
        if (resizedDimensions == null)
        {
            return null;
        }
        
        var hash:String = Utility.Md5(MangaPath) + "_" + Utility.Md5(page + "." + resizedDimensions[0] + "x" + resizedDimensions[1] + "." + part);
        var lockPath:String = "mangacache/" + hash + ".lock";
        var outputRelativePath:String = "mangacache/" + hash + ".jpg";
        var outputPath:String = outputRelativePath;

        if (CheckLockFile(lockPath))
        {
            return null;
        }
        else if (!FileSystem.exists(outputPath))
        {
            ThreadHelper.Run("MangaProcessFile", [Id, Content[page], resizedDimensions[0], resizedDimensions[1], part, outputPath, lockPath]);
            return null;
        }
        else
        {
            return outputRelativePath;
        }
    }
    
    private function CheckLockFile(lockPath:String):Bool
    {
        if (FileSystem.exists(lockPath))
        {
            if (Date.now().getTime() - FileSystem.stat(lockPath).ctime.getTime() > 180000)
            {
                try
                {
                    FileSystem.deleteFile(lockPath);
                    return false;
                }
                catch (ex:Dynamic)
                {
                    Utility.TryLogError(ex);
                }
            }
            
            return true;
        }
        
        return false;
    }
    
    private function TryOutputFile(content:String):String
    {
        var tempFilePath:String = null;
        try
        {
            tempFilePath = Provider.OutputFile(MangaPath, content, Utility.GetTempFileName());
        }
        catch (ex:FileNotFoundException)
        {
            Utility.TryLogError(ex);
            this.Status = 1;
            this.Save();
        }
        catch (ex:MangaWrongFormatException)
        {
            Utility.TryLogError(ex);
            this.Status = 2;
            this.Save();
        }
        catch (ex:MangaContentMismatchException)
        {
            Utility.TryLogError(ex);
            this.Status = 3;
            this.Save();
        }
        catch (ex:Exception)
        {
            Utility.TryLogError(ex);
            this.Status = 99;
            this.Save();
        }
        
        return tempFilePath;
    }
    
    public function ProcessFile(content:String, width:Int, height:Int, part:Int, outputPath:String, lockPath:String):Void
    {
        if (!FileSystem.exists(outputPath))
        {
            var lockFile:Dynamic = false;
            try
            {
                lockFile = untyped __call__("@fopen", lockPath, "x");
            }
            catch (ex:Dynamic)
            {
            }
            
            if (lockFile != false)
            {
                if (!FileSystem.exists(outputPath))
                {
                    var tempFilePath:String = TryOutputFile(content);
                    
                    if (tempFilePath != null)
                    {
                        ImageProvider.ResizeFile(tempFilePath, outputPath, width, height, part);
                        FileSystem.deleteFile(tempFilePath);
                    }
                }
                
                untyped __call__("@fclose", lockFile);
                FileSystem.deleteFile(lockPath);
                ThreadHelper.Run("MangaCacheLimit", []);
            }
        }
    }
    
    public function GetDimensions(page:Int):Array<Int>
    {
        if (Dimensions[page] == null)
        {
            var tempFilePath:String = TryOutputFile(Content[page]);

            if (tempFilePath != null)
            {
                Dimensions[page] = ImageProvider.GetDimensions(tempFilePath);
                FileSystem.deleteFile(tempFilePath);
                Save();
            }
        }

        return Dimensions[page];
    }
    
    private function GetResizedDimensions(page:Int, width:Int, height:Int):Array<Int>
    {
        var dimensions:Array<Int> = GetDimensions(page);
        if (dimensions != null)
        {
            var widthFactor:Float = width > 0 ? width / dimensions[0] : 1;
            var heightFactor:Float = height > 0 ? height / dimensions[1] : 1;
            widthFactor = widthFactor > 1 ? 1 : widthFactor;
            heightFactor = heightFactor > 1 ? 1 : heightFactor;
            var factor:Float = widthFactor > heightFactor ? heightFactor : widthFactor;
            return [Math.round(dimensions[0] * factor), Math.round(dimensions[1] * factor)];
        }
            
        return null;
    }
    
    private function DeleteCache():Void
    {
        var hash:String = Utility.Md5(MangaPath);
        for (file in FileSystem.readDirectory("cover/"))
        {
            if (file.indexOf(hash) == 0)
            {
                FileSystem.deleteFile("cover/" + file);
            }
        }
        
        for (file in FileSystem.readDirectory("mangacache/"))
        {
            if (file.indexOf(hash) == 0)
            {
                FileSystem.deleteFile("mangacache/" + file);
            }
        }
    }
    
    public static function CacheLimit():Void
    {
        var files:Array<Array<Dynamic>> = new Array<Array<Dynamic>>();
        for (file in FileSystem.readDirectory("mangacache/"))
        {
            if (file.indexOf(".jpg") == file.length - 4 && !FileSystem.isDirectory("mangacache/" + file))
            {
                files.push([file, FileSystem.stat("mangacache/" + file)]);
            }
        }
        
        files.sort(function(x:Array<Dynamic>, y:Array<Dynamic>):Int
        {
            var xStat:FileStat = x[1];
            var yStat:FileStat = y[1];
            return Math.round(yStat.mtime.getTime() - xStat.mtime.getTime()); 
        });
        
        var totalSize:Int = 0;
        var sizeLimit:Int = 1048576 * Settings.MangaCacheSizeLimit; // 1048576 = 1MB
        for (i in 0...files.length)
        {
            var fileStat:FileStat = files[i][1];
            if (totalSize > sizeLimit || (totalSize += fileStat.size) > sizeLimit)
            {
                FileSystem.deleteFile("mangacache/" + files[i][0]);
            }
        }
    }
    
    public function IncreaseViewCount():Void
    {
        View++;
        Save();
    }
}