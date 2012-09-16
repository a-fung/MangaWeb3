using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server
{
    public class Config
    {
        private static NameValueCollection __section = null;

        private static NameValueCollection AppSettings
        {
            get
            {
                if (__section == null)
                {
                    ConfigurationManager.RefreshSection("appSettings");
                    __section = (NameValueCollection)ConfigurationManager.GetSection("appSettings");
                }
                return __section;
            }
        }

        public static bool IsInstalled
        {
            get
            {
                string installed = AppSettings["MangaWebInstalled"];
                return String.IsNullOrWhiteSpace(installed) ? false : Boolean.Parse(installed);
            }
        }

        public static string MySQLServer
        {
            get
            {
                return AppSettings["MangaWebMySQLServer"];
            }
        }

        public static int MySQLPort
        {
            get
            {
                string port = AppSettings["MangaWebMySQLPort"];
                return String.IsNullOrWhiteSpace(port) ? 0 : int.Parse(port);
            }
        }

        public static string MySQLUser
        {
            get
            {
                return AppSettings["MangaWebMySQLUser"];
            }
        }

        public static string MySQLPassword
        {
            get
            {
                return AppSettings["MangaWebMySQLPassword"];
            }
        }

        public static string MySQLDatabase
        {
            get
            {
                return AppSettings["MangaWebMySQLDatabase"];
            }
        }

        public static string SevenZipDllPath
        {
            get
            {
                return AppSettings["MangaWeb7zDll"];
            }
        }

        public static string PdfinfoPath
        {
            get
            {
                return AppSettings["MangaWebPdfinfo"];
            }
        }

        public static string MudrawPath
        {
            get
            {
                return AppSettings["MangaWebMudraw"];
            }
        }

        public static void Refresh(NameValueCollection settings)
        {
            __section = settings;
        }
    }
}