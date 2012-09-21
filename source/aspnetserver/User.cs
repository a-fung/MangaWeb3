using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server
{
    public class User
    {
        private int id;

        private string username;

        public string Username
        {
            get
            {
                return username;
            }
        }

        private string password;

        public bool Admin
        {
            get;
            private set;
        }

        private User()
        {
            id = -1;
        }

        public static User CreateNewUser(string username, string password, bool admin)
        {
            User newUser = new User();
            newUser.username = username;
            newUser.SetPassword(password);
            newUser.Admin = admin;

            return newUser;
        }

        private static User FromData(Dictionary<string, object> data)
        {
            User user = new User();
            user.id = Convert.ToInt32(data["id"]);
            user.username = Convert.ToString(data["username"]);
            user.password = Convert.ToString(data["password"]);
            user.Admin = Convert.ToInt32(data["admin"]) == 1;
            return user;
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
            userData.Add("username", username);
            userData.Add("password", password);
            userData.Add("admin", Admin ? 1 : 0);

            if (id == -1)
            {
                Database.Insert("user", userData);
                id = Database.LastInsertId();
            }
            else
            {
                userData.Add("id", id);
                Database.Replace("user", userData);
            }
        }
    }
}