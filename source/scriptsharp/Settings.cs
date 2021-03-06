// Settings.cs
//

using System;
using System.Collections.Generic;
using System.Serialization;
using System.Html;
using jQueryApi;

namespace afung.MangaWeb3.Client
{
    public class Settings
    {
        private const string Prefix = "afung.MangaWeb3.Client.Settings.";

        public static string UserLanguage
        {
            get
            {
                return Load("UserLanguage");
            }
            set
            {
                Save("UserLanguage", value);
            }
        }

        public static int Sort
        {
            get
            {
                return int.Parse(Load("Sort"), 10);
            }
            set
            {
                Save("Sort", value.ToString());
            }
        }

        public static int DisplayType
        {
            get
            {
                return int.Parse(Load("DisplayType"), 10);
            }
            set
            {
                Save("DisplayType", value.ToString());
            }
        }

        public static bool FixAutoDownscale
        {
            get
            {
                return Load("FixAutoDownscale") != "false";
            }
            set
            {
                Save("FixAutoDownscale", value ? "true" : "false");
            }
        }

        public static bool UseAnimation
        {
            get
            {
                if (BootstrapTransition.Support)
                {
                    return Load("UseAnimation") == "true";
                }

                return false;
            }
            set
            {
                Save("UseAnimation", value ? "true" : "false");
            }
        }

        public static int KindleRefreshDelay
        {
            get
            {
                if (!Environment.IsKindle)
                {
                    return 0;
                }
                else
                {
                    string value = Load("KindleRefreshDelay");
                    if (String.IsNullOrEmpty(value))
                    {
                        return 2;
                    }

                    return int.Parse(value, 10);
                }
            }
            set
            {
                if (!Number.IsFinite(value) || value < 0 || value > 9)
                {
                    value = 0;
                }

                Save("KindleRefreshDelay", value.ToString());
            }
        }

        public static string CurrentFolder
        {
            get
            {
                return Load("CurrentFolder");
            }
            set
            {
                Save("CurrentFolder", value);
            }
        }

        public static int SearchFolderSetting
        {
            get
            {
                return int.Parse(Load("SearchFolderSetting"), 10);
            }
            set
            {
                Save("SearchFolderSetting", value.ToString());
            }
        }

        public static int GetCurrentPage(int mangaId)
        {
            return int.Parse(Load("CurrentPage." + mangaId), 10);
        }

        public static void SetCurrentPage(int mangaId, int currentPage)
        {
            Save("CurrentPage." + mangaId, currentPage.ToString());
        }

        private static void Save(string key, string value)
        {
            if (Script.IsNullOrUndefined(Window.LocalStorage))
            {
                SetCookie(Prefix + key, value.Escape(), 365);
            }
            else
            {
                Window.LocalStorage.SetItem(Prefix + key, value);
            }
        }

        private static string Load(string key)
        {
            string s = null;
            if (Script.IsNullOrUndefined(Window.LocalStorage))
            {
                s = GetCookie(Prefix + key);
                if (Script.IsNullOrUndefined(s))
                {
                    s = null;
                }
                else
                {
                    s = s.Unescape();
                }
            }
            else
            {
                s = (string)Window.LocalStorage.GetItem(Prefix + key);
            }

            if (String.IsNullOrEmpty(s)) return null;
            return s;
        }

        private static void SetCookie(string cookieName, string value, int expire)
        {
            Date expireDate = new Date();
            expireDate.SetDate(expireDate.GetDate() + expire);
            string cookieValue = value.Escape() + "; expires=" + expireDate.ToUTCString();
            Document.Cookie = cookieName + "=" + cookieValue;
        }

        private static string GetCookie(string cookieName)
        {
            string[] cookies = Document.Cookie.Split(";");
            for (int i = 0; i < cookies.Length; i++)
            {
                int j = cookies[i].IndexOf("=");
                if (cookies[i].Substr(0, j).Trim() == cookieName)
                {
                    return cookies[i].Substr(j + 1).Unescape();
                }
            }

            return null;
        }
    }
}
