using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server
{
    public class User
    {
        public int Id
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            private set;
        }

        private string password;

        public bool Admin
        {
            get;
            private set;
        }

        private User()
        {
            Id = -1;
        }

        public static User CreateNewUser(string username, string password, bool admin)
        {
            User newUser = new User();
            newUser.Username = username;
            newUser.SetPassword(password);
            newUser.Admin = admin;

            return newUser;
        }

        private static User FromData(Dictionary<string, object> data)
        {
            User user = new User();
            user.Id = Convert.ToInt32(data["id"]);
            user.Username = Convert.ToString(data["username"]);
            user.password = Convert.ToString(data["password"]);
            user.Admin = Convert.ToInt32(data["admin"]) == 1;
            return user;
        }

        public static User GetCurrentUser(AjaxBase ajax)
        {
            return GetUser(SessionWrapper.GetUserName(ajax), null);
        }

        public static User GetUser(string username)
        {
            return GetUser(username, null);
        }

        public static User GetUser(string username, string password)
        {
            if (username != null && username != "")
            {
                Dictionary<string, object>[] resultSet = Database.Select("user", "`username`=" + Database.Quote(username) + (password == null ? "" : " AND `password`=" + Database.Quote(Utility.Md5(password))));

                if (resultSet.Length == 1)
                {
                    return FromData(resultSet[0]);
                }
            }

            return null;
        }

        public static User[] GetAllUsers()
        {
            Dictionary<string, object>[] resultSet = Database.Select("user");
            List<User> users = new List<User>();

            foreach (Dictionary<string, object> result in resultSet)
            {
                users.Add(FromData(result));
            }

            return users.ToArray();
        }

        public static bool IsAdminLoggedIn(AjaxBase ajax)
        {
            User currentUser = GetCurrentUser(ajax);
            return currentUser != null && currentUser.Admin;
        }

        public void SetPassword(string password)
        {
            this.password = Utility.Md5(password);
        }

        public bool MatchPassword(string password)
        {
            return String.Equals(this.password, Utility.Md5(password), StringComparison.InvariantCultureIgnoreCase);
        }

        public void Save()
        {
            Dictionary<string, object> userData = new Dictionary<string, object>();
            userData.Add("username", Username);
            userData.Add("password", password);
            userData.Add("admin", Admin ? 1 : 0);

            if (Id == -1)
            {
                Database.Insert("user", userData);
                Id = Database.LastInsertId();
            }
            else
            {
                userData.Add("id", Id);
                Database.Replace("user", userData);
            }
        }

        public UserJson ToJson()
        {
            UserJson obj = new UserJson();
            obj.id = Id;
            obj.username = Username;
            obj.admin = Admin;
            return obj;
        }

        public static UserJson[] ToJsonArray(User[] users)
        {
            List<UserJson> objs = new List<UserJson>();
            foreach (User user in users)
            {
                objs.Add(user.ToJson());
            }

            return objs.ToArray();
        }

        public static void DeleteUsers(int[] ids, User currentUser)
        {
            if (currentUser != null && currentUser.Id != -1)
            {
                List<int> newIds = new List<int>(ids);
                while (newIds.Remove(currentUser.Id))
                {
                }

                ids = newIds.ToArray();
            }

            Database.Delete("user", Database.BuildWhereClauseOr("id", ids));
            Database.Delete("collectionuser", Database.BuildWhereClauseOr("uid", ids));
        }

        public static void SetAdmin(int[] ids, bool admin, User currentUser)
        {
            if (currentUser != null && currentUser.Id != -1)
            {
                List<int> newIds = new List<int>(ids);
                while (newIds.Remove(currentUser.Id))
                {
                }

                ids = newIds.ToArray();
            }

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("admin", admin ? 1 : 0);
            Database.Update("user", data, Database.BuildWhereClauseOr("id", ids));
        }
    }
}