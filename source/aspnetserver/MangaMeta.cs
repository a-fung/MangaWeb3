using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        public string Title
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

            string title = manga.MangaPath.Substring(0, manga.MangaPath.LastIndexOf("."));
            title = title.Substring(title.LastIndexOf("\\"));
            newMeta.Title = title;

            newMeta.Author = newMeta.Series = newMeta.Publisher = string.Empty;
            newMeta.Volume = newMeta.Year = -1;

            return newMeta;
        }

        private static MangaMeta FromData(Dictionary<string, object> data)
        {
            MangaMeta newMeta = new MangaMeta();
            newMeta.Id = Convert.ToInt32(data["id"]);
            newMeta.Title = Convert.ToString(data["title"]);
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
            data.Add("title", Title);
            data.Add("volume", Volume);
            data.Add("series", Series);
            data.Add("year", Year);
            data.Add("publisher", Publisher);

            if (Id == -1)
            {
                Database.Insert("meta", data);
                Id = Database.LastInsertId();
            }
            else
            {
                data.Add("id", Id);
                Database.Replace("meta", data);
            }
        }
    }
}