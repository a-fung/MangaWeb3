package afung.mangaWeb3.server;

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
        title = title.substr(title.lastIndexOf("/"));
        newMeta.Title = title;

        newMeta.Author = newMeta.Series = newMeta.Publisher = "";
        newMeta.Volume = newMeta.Year = -1;

        return newMeta;
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
}