package afung.mangaWeb3.server;

import afung.mangaWeb3.common.AdminMangaMetaJson;

/**
 * ...
 * @author a-fung
 */

class MangaMeta 
{
    public var Id(default, null):Int;
    
    public var ParentManga(default, null):Manga;
    
    public var Author(default, null):String;
    
    public var Title(default, null):String;
    
    public var Volume(default, null):Int;
    
    public var Series(default, null):String;
    
    public var Year(default, null):Int;
    
    public var Publisher(default, null):String;

    private function new() 
    {
        Id = -1;
    }
    
    public static function CreateNewMeta(manga:Manga):MangaMeta
    {
        var newMeta:MangaMeta = new MangaMeta();
        newMeta.ParentManga = manga;

        var title:String = manga.MangaPath.substr(0, manga.MangaPath.lastIndexOf("."));
        title = title.substr(title.lastIndexOf("/") + 1);
        newMeta.Title = title;

        newMeta.Author = newMeta.Series = newMeta.Publisher = "";
        newMeta.Volume = newMeta.Year = -1;

        return newMeta;
    }
    
    private static function FromData(data:Hash<Dynamic>):MangaMeta
    {
        var newMeta:MangaMeta = new MangaMeta();
        newMeta.Id = Std.parseInt(data.get("id"));
        newMeta.Title = Std.string(data.get("title"));
        newMeta.Author = Std.string(data.get("author"));
        newMeta.Series = Std.string(data.get("series"));
        newMeta.Publisher = Std.string(data.get("publisher"));
        newMeta.Volume = Std.parseInt(data.get("volume"));
        newMeta.Year = Std.parseInt(data.get("year"));
        return newMeta;
    }
    
    public static function Get(manga:Manga):MangaMeta
    {
        var resultSet:Array<Hash<Dynamic>> = Database.Select("meta", "`mid`=" + Database.Quote(Std.string(manga.Id)));
        
        if (resultSet.length > 0)
        {
            var newMeta:MangaMeta = FromData(resultSet[0]);
            newMeta.ParentManga = manga;
            return newMeta;
        }
        
        return null;
    }
    
    public function Save():Void
    {
        var data:Hash<Dynamic> = new Hash<Dynamic>();
        data.set("mid", ParentManga.Id);
        data.set("author", Author);
        data.set("title", Title);
        data.set("volume", Volume);
        data.set("series", Series);
        data.set("year", Year);
        data.set("publisher", Publisher);

        if (Id == -1)
        {
            Database.Insert("meta", data);
            Id = Database.LastInsertId();
        }
        else
        {
            data.set("id", Id);
            Database.Replace("meta", data);
        }
    }
    
    public function Update(obj:AdminMangaMetaJson):Void
    {
        Author = obj.author;
        Title = obj.title;
        Volume = obj.volume;
        Series = obj.series;
        Year = obj.year;
        Publisher = obj.publisher;
        Save();
    }
    
    public static function GetAuthors():Array<String>
    {
        return Database.GetDistinctStringValue("meta", "author");
    }
    
    public static function GetSeries():Array<String>
    {
        return Database.GetDistinctStringValue("meta", "series");
    }
    
    public static function GetPublishers():Array<String>
    {
        return Database.GetDistinctStringValue("meta", "publisher");
    }
}