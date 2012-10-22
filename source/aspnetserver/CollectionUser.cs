// CollectionUser.cs
// MangaWeb3 Project
// Copyright 2012 Man Kwan Liu

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server
{
    /// <summary>
    /// The CollectionUser class
    /// </summary>
    public class CollectionUser
    {
        /// <summary>
        /// Whether this object is new (not yet saved to database)
        /// </summary>
        private bool isNew = false;

        /// <summary>
        /// Collection ID
        /// </summary>
        public int CollectionId
        {
            get;
            private set;
        }

        /// <summary>
        /// User ID
        /// </summary>
        public int UserId
        {
            get;
            private set;
        }

        /// <summary>
        /// Allow/deny access
        /// </summary>
        public bool Access
        {
            get;
            private set;
        }

        /// <summary>
        /// Instantiate a new instance of CollectionUser class
        /// </summary>
        private CollectionUser()
        {
        }

        /// <summary>
        /// Create a new CollectionUser object
        /// </summary>
        /// <param name="collection">The collection</param>
        /// <param name="user">The user</param>
        /// <param name="access">Allow/deny access</param>
        /// <returns>The new CollectionUser object</returns>
        public static CollectionUser CreateNew(Collection collection, User user, bool access)
        {
            CollectionUser cu = new CollectionUser();
            cu.CollectionId = collection.Id;
            cu.UserId = user.Id;
            cu.Access = access;
            cu.isNew = true;
            return cu;
        }

        /// <summary>
        /// Create a new instance of CollectionUser using data from database
        /// </summary>
        /// <param name="data">The data</param>
        /// <returns>A CollectionUser object from data</returns>
        private static CollectionUser FromData(Dictionary<string, object> data)
        {
            CollectionUser cu = new CollectionUser();
            cu.CollectionId = Convert.ToInt32(data["cid"]);
            cu.UserId = Convert.ToInt32(data["uid"]);
            cu.Access = Convert.ToInt32(data["access"]) == 1;
            return cu;
        }

        /// <summary>
        /// Get all CollectionUser objects associated to a collection
        /// </summary>
        /// <param name="collection">The collection</param>
        /// <returns>An array of CollectionUser objects</returns>
        public static CollectionUser[] GetByCollection(Collection collection)
        {
            if (collection != null && collection.Id != -1)
            {
                return GetMultiple("`cid`=" + Database.Quote(collection.Id.ToString()));
            }

            return new CollectionUser[] { };
        }

        /// <summary>
        /// Get all CollectionUser objects associated to a user
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>An array of CollectionUser objects</returns>
        public static CollectionUser[] GetByUser(User user)
        {
            if (user != null && user.Id != -1)
            {
                return GetMultiple("`uid`=" + Database.Quote(user.Id.ToString()));
            }

            return new CollectionUser[] { };
        }

        private static CollectionUser[] GetMultiple(string where)
        {
            Dictionary<string, object>[] resultSet = Database.Select("collectionuser", where);
            List<CollectionUser> cus = new List<CollectionUser>();

            foreach (Dictionary<string, object> result in resultSet)
            {
                cus.Add(FromData(result));
            }

            return cus.ToArray();
        }

        public static CollectionUser Get(Collection collection, User user)
        {
            CollectionUser[] cus = GetMultiple("`cid`=" + Database.Quote(collection.Id.ToString()) + " AND `uid`=" + Database.Quote(user.Id.ToString()));
            if (cus.Length > 0)
            {
                return cus[0];
            }

            return null;
        }

        public void Save()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("cid", CollectionId);
            data.Add("uid", UserId);
            data.Add("access", Access ? 1 : 0);

            if (isNew)
            {
                Database.Insert("collectionuser", data);
            }
            else
            {
                Database.Update("collectionuser", data, "`cid`=" + Database.Quote(CollectionId.ToString()) + " AND `uid`=" + Database.Quote(UserId.ToString()));
            }
        }

        public CollectionUserJson ToJson()
        {
            CollectionUserJson obj = new CollectionUserJson();
            obj.cid = CollectionId;
            obj.uid = UserId;
            obj.access = Access;
            obj.collectionName = Collection.GetById(CollectionId).Name;
            obj.username = User.GetById(UserId).Username;
            return obj;
        }

        public static CollectionUserJson[] ToJsonArray(CollectionUser[] cus)
        {
            List<CollectionUserJson> objs = new List<CollectionUserJson>();
            foreach (CollectionUser cu in cus)
            {
                objs.Add(cu.ToJson());
            }

            return objs.ToArray();
        }

        public static void DeleteRelations(int t, int id, int[] ids)
        {
            string primaryId = t == 0 ? "cid" : "uid";
            string secondaryId = t == 0 ? "uid" : "cid";

            Database.Delete("collectionuser", "`" + primaryId + "`=" + Database.Quote(id.ToString()) + " AND " + Database.BuildWhereClauseOr(secondaryId, ids));
        }

        public static void SetRelationsAcess(int t, int id, int[] ids, bool access)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("access", access ? 1 : 0);

            string primaryId = t == 0 ? "cid" : "uid";
            string secondaryId = t == 0 ? "uid" : "cid";

            Database.Update("collectionuser", data, "`" + primaryId + "`=" + Database.Quote(id.ToString()) + " AND " + Database.BuildWhereClauseOr(secondaryId, ids));
        }
    }
}