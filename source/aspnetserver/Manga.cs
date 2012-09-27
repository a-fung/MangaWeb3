using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server
{
    public class Manga
    {
        public int Id
        {
            get;
            private set;
        }

        public Collection ParentCollection
        {
            get;
            private set;
        }

        public string MangaPath
        {
            get;
            private set;
        }

        public int MangaType
        {
            get;
            private set;
        }

        public string Content
        {
            get;
            private set;
        }

        public int View
        {
            get;
            private set;
        }

        public int Status
        {
            get;
            private set;
        }

        private Manga()
        {
            Id = -1;
        }

        public static Manga CreateNewManga(Collection collection, string path)
        {
            Manga newManga = new Manga();
            newManga.ParentCollection = collection;
            newManga.MangaPath = path;
            return newManga;
        }

        public static string CheckMangaPath(string path)
        {
            path = Utility.GetFullPath(path);

            if (path == null || !File.Exists(path))
            {
                return null;
            }

            return path;
        }

        public static int CheckMangaType(string path)
        {
            string extension = Path.GetExtension(path).ToLowerInvariant();

            if (Settings.UseZip && extension == ".zip")
            {
                return 0;
            }

            if (Settings.UseRar && extension == ".rar")
            {
                return 1;
            }

            if (Settings.UsePdf && extension == ".pdf")
            {
                return 2;
            }

            return -1;
        }
    }
}