using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using afung.MangaWeb3.Common;

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
            return newCollection;
        }

        private static Collection FromData(Dictionary<string, object> data)
        {
            Collection collection = new Collection();
            collection.Id = Convert.ToInt32(data["id"]);
            collection.Name = Convert.ToString(data["name"]);
            collection.Path = Convert.ToString(data["path"]);
            collection.Public = Convert.ToInt32(data["public"]) == 1;
            collection.AutoAdd = Convert.ToInt32(data["autoadd"]) == 1;
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
            Dictionary<string, object>[] resultSet = Database.Select("collection", "`id`=" + Database.Quote(id.ToString()));

            if (resultSet.Length > 0)
            {
                return FromData(resultSet[0]);
            }

            return null;
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

            if (Id == -1)
            {
                Database.Insert("collection", data);
                Id = Database.LastInsertId();
            }
            else
            {
                data.Add("id", Id);
                Database.Replace("collection", data);
            }
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
            Database.Delete("collection", Database.BuildWhereClauseOr("id", ids));
            Database.Delete("collectionuser", Database.BuildWhereClauseOr("cid", ids));

            // todo: delete mangas
        }

        public static void SetCollectionsPublic(int[] ids, bool public_)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("public", public_ ? 1 : 0);
            Database.Update("collection", data, Database.BuildWhereClauseOr("id", ids));
        }
    }
}