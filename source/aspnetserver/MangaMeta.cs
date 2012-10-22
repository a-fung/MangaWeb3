using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server
{
    public class MangaMeta
    {
        public int Id
        {
            get;
            private set;
        }

        public Manga ParentManga
        {
            get;
            private set;
        }

        public string Author
        {
            get;
            private set;
        }

        public int Volume
        {
            get;
            private set;
        }

        public string Series
        {
            get;
            private set;
        }

        public int Year
        {
            get;
            private set;
        }

        public string Publisher
        {
            get;
            private set;
        }

        private MangaMeta()
        {
            Id = -1;
        }

        public static MangaMeta CreateNewMeta(Manga manga)
        {
            MangaMeta newMeta = new MangaMeta();
            newMeta.ParentManga = manga;

            newMeta.Author = newMeta.Series = newMeta.Publisher = string.Empty;
            newMeta.Volume = newMeta.Year = -1;

            return newMeta;
        }

        private static MangaMeta FromData(Dictionary<string, object> data)
        {
            MangaMeta newMeta = new MangaMeta();
            newMeta.Id = Convert.ToInt32(data["id"]);
            newMeta.Author = Convert.ToString(data["author"]);
            newMeta.Series = Convert.ToString(data["series"]);
            newMeta.Publisher = Convert.ToString(data["publisher"]);
            newMeta.Volume = Convert.ToInt32(data["volume"]);
            newMeta.Year = Convert.ToInt32(data["year"]);
            return newMeta;
        }

        public static MangaMeta Get(Manga manga)
        {
            Dictionary<string, object>[] resultSet = Database.Select("meta", "`mid`=" + Database.Quote(manga.Id.ToString()));

            if (resultSet.Length > 0)
            {
                MangaMeta newMeta = FromData(resultSet[0]);
                newMeta.ParentManga = manga;
                return newMeta;
            }

            return null;
        }

        public void Save()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("mid", ParentManga.Id);
            data.Add("author", Author);
            data.Add("volume", Volume);
            data.Add("series", Series);
            data.Add("year", Year);
            data.Add("publisher", Publisher);

            if (Id == -1)
            {
                Id = Database.InsertAndReturnId("meta", data);
            }
            else
            {
                data.Add("id", Id);
                Database.Replace("meta", data);
            }
        }

        public void Update(AdminMangaMetaJson obj)
        {
            Author = Utility.Remove4PlusBytesUtf8Chars(obj.author);
            Volume = obj.volume;
            Series = Utility.Remove4PlusBytesUtf8Chars(obj.series);
            Year = obj.year;
            Publisher = Utility.Remove4PlusBytesUtf8Chars(obj.publisher);
            Save();
        }

        public static string[] GetAuthors()
        {
            return Database.GetDistinctStringValues("meta", "author");
        }

        public static string[] GetSeries()
        {
            return Database.GetDistinctStringValues("meta", "series");
        }

        public static string[] GetPublishers()
        {
            return Database.GetDistinctStringValues("meta", "publisher");
        }

        public void Copy(MangaMeta other)
        {
            Author = other.Author;
            Volume = other.Volume;
            Series = other.Series;
            Year = other.Year;
            Publisher = other.Publisher;
            Save();
        }
    }
}