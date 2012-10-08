﻿using System;
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

        private MangaMeta _meta = null;

        public MangaMeta Meta
        {
            get
            {
                if (_meta == null && Id != -1)
                {
                    _meta = MangaMeta.Get(this);
                }

                return _meta;
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
            newManga.InnerRefreshContent();
            newManga.View = newManga.Status = 0;
            newManga._meta = MangaMeta.CreateNewMeta(newManga);
            return newManga;
        }

        private static Manga FromData(Dictionary<string, object> data)
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

        public static Manga[] GetMangasWithFilter(Collection collection, string tag, string author, int type)
        {
            string where = "TRUE";

            if (collection != null)
            {
                where += " AND `cid`=" + Database.Quote(collection.Id.ToString());
            }

            if (!String.IsNullOrEmpty(tag))
            {
                where += " AND `id` IN (SELECT `mid` FROM `mangatag` WHERE `tid` IN (SELECT `id` FROM `tag` WHERE `name`=" + Database.Quote(tag) + "))";
            }

            if (!String.IsNullOrEmpty(author))
            {
                where += " AND `id` IN (SELECT `mid` FROM `meta` WHERE `author`=" + Database.Quote(author) + ")";
            }

            if (type != -1)
            {
                where += " AND `type`=" + Database.Quote(type.ToString());
            }

            return GetMangas(where);
        }

        public static Manga[] GetMangaList(AjaxBase ajax, MangaFilter filter)
        {
            string where = "TRUE";
            User user = User.GetCurrentUser(ajax);
            string collectionSelect = "FALSE";
            if (Settings.AllowGuest || user != null)
            {
                collectionSelect += " OR `cid` IN (SELECT `id` FROM `collection` WHERE `public`='1')";
            }

            if (user != null)
            {
                collectionSelect += " OR `cid` IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(user.Id.ToString()) + " AND `access`='1')";
                where += " AND `cid` NOT IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(user.Id.ToString()) + " AND `access`='0')";
            }

            where += " AND (" + collectionSelect + ")";

            if (filter != null)
            {

            }

            return GetMangas(where);
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

        public MangaListItemJson ToMangaListItemJson()
        {
            MangaListItemJson obj = new MangaListItemJson();
            obj.id = Id;
            obj.title = Meta.Title;
            obj.pages = NumberOfPages;
            obj.size = Size;
            return obj;
        }

        public MangaJson ToJson()
        {
            MangaJson obj = new MangaJson();
            obj.id = Id;
            obj.title = Meta.Title;
            obj.collection = ParentCollection.Name;
            obj.path = MangaPath;
            obj.type = MangaType;
            obj.view = View;
            obj.status = Status;
            return obj;
        }

        public AdminMangaMetaJson GetMetaJson()
        {
            AdminMangaMetaJson obj = new AdminMangaMetaJson();
            obj.author = Meta.Author;
            obj.title = Meta.Title;
            obj.volume = Meta.Volume;
            obj.series = Meta.Series;
            obj.year = Meta.Year;
            obj.publisher = Meta.Publisher;
            obj.tags = GetTags();
            return obj;
        }

        public void UpdateMeta(AdminMangaMetaJson obj)
        {
            Meta.Update(obj);
            UpdateTags(obj.tags);
        }

        public static MangaListItemJson[] ToListItemJsonArray(Manga[] mangas)
        {
            List<MangaListItemJson> objs = new List<MangaListItemJson>();
            foreach (Manga manga in mangas)
            {
                objs.Add(manga.ToMangaListItemJson());
            }

            return objs.ToArray();
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
            UpdateTags(new string[0]);
            Database.Delete("manga", "`id`=" + Database.Quote(Id.ToString()));
            Database.Delete("meta", "`mid`=" + Database.Quote(Id.ToString()));
        }

        public static string[] GetAllTags()
        {
            return Database.GetDistinctStringValues("tag", "name");
        }

        private string[] GetTags()
        {
            return Database.GetDistinctStringValues("tag", "name", "`id` IN (SELECT `tid` FROM `mangatag` WHERE `mid`=" + Database.Quote(Id.ToString()) + ")");
        }

        private void UpdateTags(string[] tags)
        {
            int id;
            string[] oldTags = GetTags();
            string[] allTags = GetAllTags();

            foreach (string rawTag in tags)
            {
                string tag = Utility.Remove4PlusBytesUtf8Chars(rawTag);

                if (!oldTags.Contains(tag, StringComparer.InvariantCultureIgnoreCase))
                {
                    if (allTags.Contains(tag, StringComparer.InvariantCultureIgnoreCase))
                    {
                        id = Convert.ToInt32(Database.Select("tag", "`name`=" + Database.Quote(tag))[0]["id"]);
                    }
                    else
                    {
                        Dictionary<string, object> tagData = new Dictionary<string, object>();
                        tagData["name"] = tag;
                        Database.Insert("tag", tagData);
                        id = Database.LastInsertId();
                    }

                    Dictionary<string, object> mangaTagData = new Dictionary<string, object>();
                    mangaTagData["tid"] = id; // tag ID
                    mangaTagData["mid"] = Id; // manga ID
                    Database.Insert("mangatag", mangaTagData);
                }
            }

            foreach (string tag in oldTags)
            {
                if (!tags.Contains(tag, StringComparer.InvariantCultureIgnoreCase))
                {
                    id = Convert.ToInt32(Database.Select("tag", "`name`=" + Database.Quote(tag))[0]["id"]);
                    Database.Delete("mangatag", "`tid`=" + Database.Quote(id.ToString()) + " AND `mid`=" + Database.Quote(Id.ToString()));
                    if (Database.Select("mangatag", "`tid`=" + Database.Quote(id.ToString())).Length == 0)
                    {
                        Database.Delete("tag", "`id`=" + Database.Quote(id.ToString()));
                    }
                }
            }
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

        public string GetCover()
        {
            string hash = Utility.Md5(MangaPath);
            string lockPath = Path.Combine(AjaxBase.DirectoryPath, "cover", hash + ".lock");
            string coverRelativePath = Path.Combine("cover", hash + ".jpg");
            string coverPath = Path.Combine(AjaxBase.DirectoryPath, coverRelativePath);

            if (File.Exists(lockPath))
            {
                return null;
            }
            else if (!File.Exists(coverPath))
            {
                ThreadHelper.Run("MangaProcessFile", Id, Content[0], 260, 200, coverPath, lockPath);
                return null;
            }
            else
            {
                return coverRelativePath;
            }
        }

        public void ProcessFile(string content, int width, int height, string outputPath, string lockPath)
        {
            if (!File.Exists(outputPath))
            {
                FileStream lockFile;

                try
                {
                    lockFile = File.Open(lockPath, FileMode.CreateNew);
                }
                catch (IOException)
                {
                    return;
                }

                if (!File.Exists(outputPath))
                {
                    string tempFilePath = null;

                    try
                    {
                        tempFilePath = Provider.OutputFile(MangaPath, content, Utility.GetTempFileName());
                    }
                    catch (InvalidOperationException exception)
                    {
                        this.Status = (int)exception.Data["manga_status"];
                        this.Save();
                    }

                    if (tempFilePath != null)
                    {
                        ImageProvider.ResizeFile(tempFilePath, outputPath, width, height);
                        File.Delete(tempFilePath);
                    }
                }

                lockFile.Close();
                File.Delete(lockPath);
            }
        }
    }
}