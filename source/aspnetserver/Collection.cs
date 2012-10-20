using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using afung.MangaWeb3.Common;
using Newtonsoft.Json;

namespace afung.MangaWeb3.Server
{
    public class Collection
    {
        public int Id
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Path
        {
            get;
            private set;
        }

        public bool Public
        {
            get;
            private set;
        }

        public bool AutoAdd
        {
            get;
            private set;
        }

        public int CacheStatus
        {
            get;
            private set;
        }

        private string _folderCache;

        public string FolderCache
        {
            get
            {
                if (CacheStatus == 0)
                {
                    if (_folderCache == null)
                    {
                        _folderCache = Convert.ToString(Database.Select("foldercache", "`id`='" + Id + "'")[0]["content"]);
                    }

                    return _folderCache;
                }

                return null;
            }
        }

        private static Dictionary<int, Collection> cache = new Dictionary<int, Collection>();

        private Collection()
        {
            Id = -1;
        }

        public static Collection CreateNewCollection(string name, string path, bool public_, bool autoadd)
        {
            Collection newCollection = new Collection();
            newCollection.Name = name;
            newCollection.Path = path;
            newCollection.Public = public_;
            newCollection.AutoAdd = autoadd;
            newCollection.CacheStatus = 1;
            return newCollection;
        }

        private static Collection FromData(Dictionary<string, object> data)
        {
            int id = Convert.ToInt32(data["id"]);
            Collection collection;
            if (cache.TryGetValue(id, out collection))
            {
                return collection;
            }

            collection = new Collection();
            collection.Id = id;
            collection.Name = Convert.ToString(data["name"]);
            collection.Path = Convert.ToString(data["path"]);
            collection.Public = Convert.ToInt32(data["public"]) == 1;
            collection.AutoAdd = Convert.ToInt32(data["autoadd"]) == 1;
            collection.CacheStatus = Convert.ToInt32(data["cachestatus"]);

            cache[collection.Id] = collection;
            return collection;
        }

        public static Collection GetByName(string name)
        {
            if (name != null && name != "")
            {
                Dictionary<string, object>[] resultSet = Database.Select("collection", "`name`=" + Database.Quote(name));

                if (resultSet.Length > 0)
                {
                    return FromData(resultSet[0]);
                }
            }

            return null;
        }

        public static Collection GetById(int id)
        {
            Collection collection;
            if (cache.TryGetValue(id, out collection))
            {
                return collection;
            }

            Dictionary<string, object>[] resultSet = Database.Select("collection", "`id`=" + Database.Quote(id.ToString()));

            if (resultSet.Length > 0)
            {
                return FromData(resultSet[0]);
            }

            return null;
        }

        public static Collection[] GetAccessible(AjaxBase ajax)
        {
            User user = User.GetCurrentUser(ajax);
            string where = "FALSE";
            if (Settings.AllowGuest || user != null)
            {
                where += " OR `public`='1'";
            }

            if (user != null)
            {
                where += " OR `id` IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(user.Id.ToString()) + " AND `access`='1')";
            }

            Dictionary<string, object>[] resultSet = Database.Select("collection", where);
            List<Collection> collections = new List<Collection>();

            foreach (Dictionary<string, object> result in resultSet)
            {
                collections.Add(FromData(result));
            }

            return collections.ToArray();
        }

        public static Collection[] GetAutoAdd()
        {
            Dictionary<string, object>[] resultSet = Database.Select("collection", "`autoadd`='1'");
            List<Collection> collections = new List<Collection>();

            foreach (Dictionary<string, object> result in resultSet)
            {
                collections.Add(FromData(result));
            }

            return collections.ToArray();
        }

        public static Collection[] GetAllCollections()
        {
            Dictionary<string, object>[] resultSet = Database.Select("collection");
            List<Collection> collections = new List<Collection>();

            foreach (Dictionary<string, object> result in resultSet)
            {
                collections.Add(FromData(result));
            }

            return collections.ToArray();
        }

        public static string[] GetAllCollectionNames()
        {
            return Database.GetDistinctStringValues("collection", "name");
        }

        public static bool CheckNewCollectionName(string name)
        {
            return GetByName(name) == null;
        }

        public static string CheckNewCollectionPath(string path)
        {
            path = Utility.GetFullPath(path);

            if (path == null || !Directory.Exists(path))
            {
                return null;
            }

            if (path[path.Length - 1] != '\\')
            {
                path += "\\";
            }

            Collection[] collections = GetAllCollections();
            StringComparison sc = StringComparison.InvariantCultureIgnoreCase;

            foreach (Collection collection in collections)
            {
                if (path.Equals(collection.Path, sc) || path.IndexOf(collection.Path, sc) == 0 || collection.Path.IndexOf(path, sc) == 0)
                {
                    return null;
                }
            }

            return path;
        }

        public void Save()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("name", Name);
            data.Add("path", Path);
            data.Add("public", Public ? 1 : 0);
            data.Add("autoadd", AutoAdd ? 1 : 0);
            data.Add("cachestatus", CacheStatus);

            if (Id == -1)
            {
                Id = Database.InsertAndReturnId("collection", data);
            }
            else
            {
                data.Add("id", Id);
                Database.Replace("collection", data);
            }

            cache[Id] = this;
        }

        public CollectionJson ToJson()
        {
            CollectionJson obj = new CollectionJson();
            obj.id = Id;
            obj.name = Name;
            obj.path = Path;
            obj.public_ = Public;
            obj.autoadd = AutoAdd;
            return obj;
        }

        public static CollectionJson[] ToJsonArray(Collection[] collections)
        {
            List<CollectionJson> objs = new List<CollectionJson>();
            foreach (Collection collection in collections)
            {
                objs.Add(collection.ToJson());
            }

            return objs.ToArray();
        }

        public static void DeleteCollections(int[] ids)
        {
            Manga.DeleteMangasFromCollectionIds(ids);
            Database.Delete("collection", Database.BuildWhereClauseOr("id", ids));
            Database.Delete("collectionuser", Database.BuildWhereClauseOr("cid", ids));
        }

        public static void SetCollectionsPublic(int[] ids, bool public_)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("public", public_ ? 1 : 0);
            Database.Update("collection", data, Database.BuildWhereClauseOr("id", ids));
        }

        public bool Accessible(AjaxBase ajax)
        {
            string where = "`id`=" + Database.Quote(Id.ToString());
            User user = User.GetCurrentUser(ajax);
            string collectionSelect = "FALSE";
            if (Settings.AllowGuest || user != null)
            {
                collectionSelect += " OR `public`='1'";
            }

            if (user != null)
            {
                collectionSelect += " OR `id` IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(user.Id.ToString()) + " AND `access`='1')";
                where += " AND `id` NOT IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(user.Id.ToString()) + " AND `access`='0')";
            }

            where += " AND (" + collectionSelect + ")";

            return Database.Select("collection", where, null, null, "`id`").Length > 0;
        }

        public void MarkFolderCacheDirty()
        {
            CacheStatus = 1;
            Save();
        }

        public void ProcessFolderCache()
        {
            if (CacheStatus == 0 || CacheStatus == 2)
            {
                return;
            }

            CacheStatus = 2;
            Save();

            FolderJson folder = new FolderJson();
            folder.name = Name;
            folder.subfolders = new FolderJson[] { };
            int collectionPathLength = Path.Length;
            string separator = "\\";

            Dictionary<string, FolderJson> folderDictionary = new Dictionary<string, FolderJson>();
            Dictionary<string, object>[] resultSet = Database.Select("manga", "`cid`=" + Database.Quote(Id.ToString()), null, null, "`path`");
            folderDictionary[""] = folder;

            foreach (Dictionary<string, object> result in resultSet)
            {
                string path = Convert.ToString(result["path"]).Substring(collectionPathLength);
                int i = 0, j = 0;

                while ((i = path.IndexOf(separator, j)) != -1)
                {
                    string relativePath = path.Substring(0, i);
                    if (!folderDictionary.ContainsKey(relativePath.ToLowerInvariant()))
                    {
                        FolderJson subfolder = new FolderJson();
                        subfolder.name = path.Substring(j, i - j);
                        subfolder.subfolders = new FolderJson[] { };
                        folderDictionary[relativePath.ToLowerInvariant()] = subfolder;

                        int k;
                        FolderJson parentFolder = folderDictionary[(k = relativePath.LastIndexOf(separator)) == -1 ? "" : relativePath.Substring(0, k).ToLowerInvariant()];
                        FolderJson[] newSubfolders = new FolderJson[parentFolder.subfolders.Length + 1];
                        Array.Copy(parentFolder.subfolders, newSubfolders, parentFolder.subfolders.Length);
                        newSubfolders[parentFolder.subfolders.Length] = subfolder;
                        parentFolder.subfolders = newSubfolders;
                    }

                    j = i + 1;
                }
            }

            Dictionary<string, object> cacheData = new Dictionary<string, object>();
            cacheData.Add("id", Id);
            cacheData.Add("content", _folderCache = JsonConvert.SerializeObject(folder));
            Database.Replace("foldercache", cacheData);

            CacheStatus = 0;
            Save();
        }
    }
}