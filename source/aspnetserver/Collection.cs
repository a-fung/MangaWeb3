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
    /// <summary>
    /// The Collection class
    /// </summary>
    public class Collection
    {
        /// <summary>
        /// ID of the collection
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Name of the collection
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Path of the collection
        /// </summary>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether the collection is public
        /// </summary>
        public bool Public
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether the collection uses auto add
        /// </summary>
        public bool AutoAdd
        {
            get;
            private set;
        }

        /// <summary>
        /// The folder cache status of the collection
        /// </summary>
        public int CacheStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// The folder cache in json string
        /// </summary>
        private string _folderCache;

        /// <summary>
        /// Getter the folder cache
        /// </summary>
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

        /// <summary>
        /// The cache of collections
        /// </summary>
        private static Dictionary<int, Collection> cache = new Dictionary<int, Collection>();

        /// <summary>
        /// Instantiate a new instance of Collection class
        /// </summary>
        private Collection()
        {
            Id = -1;
        }

        /// <summary>
        /// Create a new collection
        /// </summary>
        /// <param name="name">The collection name</param>
        /// <param name="path">The collection path</param>
        /// <param name="public_">Whether the collection is public</param>
        /// <param name="autoadd">Whether the collection uses auto add</param>
        /// <returns>A new collection</returns>
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

        /// <summary>
        /// Create a new instance of Collection using data from database
        /// </summary>
        /// <param name="data">The data</param>
        /// <returns>A collection from data</returns>
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

        /// <summary>
        /// Get a collection from database by name
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>A collection object or null if not found</returns>
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

        /// <summary>
        /// Get a collection from database by ID
        /// </summary>
        /// <param name="name">The ID</param>
        /// <returns>A collection object or null if not found</returns>
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

        /// <summary>
        /// Get a list of collections from database which is accessible by the current user
        /// </summary>
        /// <param name="ajax">The AjaxBase object which received the request</param>
        /// <returns>A list of collections</returns>
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

        /// <summary>
        /// Get a list of collections from database which uses auto add
        /// </summary>
        /// <returns>A list of collections</returns>
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

        /// <summary>
        /// Get all collections from database
        /// </summary>
        /// <returns>All collections</returns>
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

        /// <summary>
        /// Get the names of all the collections from database
        /// </summary>
        /// <returns>An array of string containing all the names</returns>
        public static string[] GetAllCollectionNames()
        {
            return Database.GetDistinctStringValues("collection", "name");
        }

        /// <summary>
        /// Check whether a name can be used for creating a new collection
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>True if the name is valid for new collection</returns>
        public static bool CheckNewCollectionName(string name)
        {
            return GetByName(name) == null;
        }

        /// <summary>
        /// Check whether a path can be used for creating a new collection
        /// </summary>
        /// <param name="name">The path (absolute or relative)</param>
        /// <returns>null if the path is invalid, a normalized absolute path if the path is valid</returns>
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

        /// <summary>
        /// Save the collection to database
        /// </summary>
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

        /// <summary>
        /// Get an object to be stringified and passed to client app
        /// </summary>
        /// <returns>A CollectionJson object</returns>
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

        /// <summary>
        /// Get an array of CollectionJson to be passed to client app
        /// </summary>
        /// <param name="collections">An array of collections</param>
        /// <returns>An array of CollectionJson</returns>
        public static CollectionJson[] ToJsonArray(Collection[] collections)
        {
            List<CollectionJson> objs = new List<CollectionJson>();
            foreach (Collection collection in collections)
            {
                objs.Add(collection.ToJson());
            }

            return objs.ToArray();
        }

        /// <summary>
        /// Delete collections and all associated data from database
        /// </summary>
        /// <param name="ids">A list of collection IDs</param>
        public static void DeleteCollections(int[] ids)
        {
            Manga.DeleteMangasFromCollectionIds(ids);
            Database.Delete("collection", Database.BuildWhereClauseOr("id", ids));
            Database.Delete("collectionuser", Database.BuildWhereClauseOr("cid", ids));
        }

        /// <summary>
        /// Set collections to be public/private
        /// </summary>
        /// <param name="ids">A list of collection IDs</param>
        /// <param name="public_">Whether is public or private</param>
        public static void SetCollectionsPublic(int[] ids, bool public_)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("public", public_ ? 1 : 0);
            Database.Update("collection", data, Database.BuildWhereClauseOr("id", ids));
        }

        /// <summary>
        /// Get whether the current request can access the collection
        /// </summary>
        /// <param name="ajax">The AjaxBase object which received the request</param>
        /// <returns>True if accessible</returns>
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

        /// <summary>
        /// Mark Folder Cache Dirty
        /// </summary>
        public void MarkFolderCacheDirty()
        {
            CacheStatus = 1;
            Save();
        }

        /// <summary>
        /// Process folder cache and save it in database
        /// </summary>
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