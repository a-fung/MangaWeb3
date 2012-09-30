using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Server.Provider;
using Newtonsoft.Json;

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

        public string[] Content
        {
            get;
            private set;
        }

        public int ModifiedTime
        {
            get;
            private set;
        }

        public long Size
        {
            get;
            private set;
        }

        public int NumberOfPages
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
        private IMangaProvider provider;

        private IMangaProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    switch (MangaType)
                    {
                        case 0:
                            provider = new ZipProvider();
                            break;
                        case 1:
                            provider = new RarProvider();
                            break;
                        case 2:
                            provider = new PdfProvider();
                            break;
                        default:
                            throw new InvalidOperationException("Invalid MangaType");
                    }
                }

                return provider;
            }
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
            newManga.MangaType = CheckMangaType(path);
            newManga.RefreshContent();
            newManga.View = newManga.Status = 0;
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
            string extension = Utility.GetExtension(path).ToLowerInvariant();

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

        public void RefreshContent()
        {
            FileInfo info = new FileInfo(MangaPath);
            ModifiedTime = Utility.ToUnixTimeStamp(info.LastWriteTimeUtc);
            Size = info.Length;
            Content = Provider.GetContent(MangaPath);
            NumberOfPages = Content.Length;
        }

        public void Save()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("cid", ParentCollection.Id);
            data.Add("path", MangaPath);
            data.Add("type", MangaType);
            data.Add("content", JsonConvert.SerializeObject(Content));
            data.Add("time", ModifiedTime);
            data.Add("size", Size);
            data.Add("numpages", NumberOfPages);
            data.Add("view", View);
            data.Add("status", Status);

            if (Id == -1)
            {
                Database.Insert("user", data);
                Id = Database.LastInsertId();
            }
            else
            {
                data.Add("id", Id);
                Database.Replace("user", data);
            }
        }
    }
}