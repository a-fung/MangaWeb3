using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Server.Provider;
using Newtonsoft.Json;
using afung.MangaWeb3.Common;

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

        public MangaMeta Meta
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
            newManga.MangaType = CheckMangaType(path);
            newManga.InnerRefreshContent();
            newManga.View = newManga.Status = 0;
            newManga.Meta = MangaMeta.CreateNewMeta(newManga);
            return newManga;
        }

        public static Manga FromData(Dictionary<string, object> data)
        {
            Manga newManga = new Manga();
            newManga.Id = Convert.ToInt32(data["id"]);
            newManga.ParentCollection = Collection.GetById(Convert.ToInt32(data["cid"]));
            newManga.MangaPath = Convert.ToString(data["path"]);
            newManga.MangaType = Convert.ToInt32(data["type"]);
            newManga.ModifiedTime = Convert.ToInt32(data["time"]);
            newManga.Size = Convert.ToInt64(data["size"]);
            newManga.Content = Utility.JsonDecodeArchiveContentString(Convert.ToString(data["content"]));
            newManga.NumberOfPages = Convert.ToInt32(data["numpages"]);
            newManga.View = Convert.ToInt32(data["view"]);
            newManga.Status = Convert.ToInt32(data["status"]);
            return newManga;
        }

        public static Manga GetById(int id)
        {
            Dictionary<string, object>[] resultSet = Database.Select("manga", "`id`=" + Database.Quote(id.ToString()));

            if (resultSet.Length > 0)
            {
                return FromData(resultSet[0]);
            }

            return null;
        }

        public static Manga GetByPath(string path)
        {
            if (path != null && path != "")
            {
                Dictionary<string, object>[] resultSet = Database.Select("manga", "`path`=" + Database.Quote(path));

                if (resultSet.Length > 0)
                {
                    return FromData(resultSet[0]);
                }
            }

            return null;
        }

        private static Manga[] GetMangas(string where)
        {
            Dictionary<string, object>[] resultSet = Database.Select("manga", where);
            List<Manga> mangas = new List<Manga>();

            foreach (Dictionary<string, object> result in resultSet)
            {
                mangas.Add(FromData(result));
            }

            return mangas.ToArray();
        }

        public static Manga[] GetAllMangas()
        {
            return GetMangas(null);
        }

        public static string CheckMangaPath(string path)
        {
            path = Utility.GetFullPath(path);

            if (path == null || !File.Exists(path) || GetByPath(path) != null)
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
            if (!File.Exists(MangaPath))
            {
                Status = 1;
            }
            else
            {
                InnerRefreshContent();

                if (Content.Length == 0)
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

        private void InnerRefreshContent()
        {
            FileInfo info = new FileInfo(MangaPath);
            ModifiedTime = Utility.ToUnixTimeStamp(info.LastWriteTimeUtc);
            Size = info.Length;
            Content = Provider.GetContent(MangaPath);
            NumberOfPages = Content.Length;
        }

        public void ChangePath(string newPath, int newType)
        {
            MangaPath = newPath;
            MangaType = newType;
            InnerRefreshContent();
            Status = 0;
            Save();
        }

        public void Save()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("cid", ParentCollection.Id);
            data.Add("path", MangaPath);
            data.Add("type", MangaType);
            data.Add("content", Utility.JsonEncodeArchiveContent(Content));
            data.Add("time", ModifiedTime);
            data.Add("size", Size);
            data.Add("numpages", NumberOfPages);
            data.Add("view", View);
            data.Add("status", Status);

            if (Id == -1)
            {
                Database.Insert("manga", data);
                Id = Database.LastInsertId();
                Meta.Save();
            }
            else
            {
                data.Add("id", Id);
                Database.Replace("manga", data);
            }
        }

        public MangaJson ToJson()
        {
            MangaJson obj = new MangaJson();
            obj.id = Id;
            obj.collection = ParentCollection.Name;
            obj.path = MangaPath;
            obj.type = MangaType;
            obj.view = View;
            obj.status = Status;
            return obj;
        }

        public static MangaJson[] ToJsonArray(Manga[] mangas)
        {
            List<MangaJson> objs = new List<MangaJson>();
            foreach (Manga manga in mangas)
            {
                objs.Add(manga.ToJson());
            }

            return objs.ToArray();
        }

        public void Delete()
        {
            Database.Delete("manga", "`id`=" + Database.Quote(Id.ToString()));
            Database.Delete("meta", "`mid`=" + Database.Quote(Id.ToString()));

            // TODO: delete tags
        }

        public static void DeleteMangas(Manga[] mangas)
        {
            foreach (Manga manga in mangas)
            {
                manga.Delete();
            }
        }

        public static void DeleteMangasFromCollectionIds(int[] cids)
        {
            DeleteMangas(GetMangas(Database.BuildWhereClauseOr("cid", cids)));
        }

        public static void DeleteMangasFromIds(int[] ids)
        {
            DeleteMangas(GetMangas(Database.BuildWhereClauseOr("id", ids)));
        }

        public static void RefreshMangasContent(int[] ids)
        {
            foreach (Manga manga in GetMangas(Database.BuildWhereClauseOr("id", ids)))
            {
                manga.RefreshContent();
            }
        }
    }
}