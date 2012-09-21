using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server
{
    public class SessionWrapper
    {
        public static string GetUserName(AjaxBase ajax)
        {
            object username = Get(ajax, "username");
            if (username == null)
            {
                return "";
            }
            else
            {
                return Convert.ToString(username);
            }
        }

        public static void SetUserName(AjaxBase ajax, string value)
        {
            if (value == null)
            {
                value = "";
            }

            Set(ajax, "username", value);
        }

        private static object Get(AjaxBase ajax, string name)
        {
            return ajax.Session["afung.MangaWeb3.Server.Session." + name];
        }

        private static void Set(AjaxBase ajax, string name, object value)
        {
            ajax.Session["afung.MangaWeb3.Server.Session." + name] = value;
        }
    }
}