using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server
{
    public class Config
    {
        public static bool IsInstalled
        {
            get
            {
                string installed = ConfigurationManager.AppSettings["MangaWebInstalled"];
                return String.IsNullOrWhiteSpace(installed) ? false : Boolean.Parse(installed);
            }
        }

        public static string MySQLServer
        {
            get
            {
                return ConfigurationManager.AppSettings["MangaWebMySQLServer"];
            }
        }

        public static int MySQLPort
        {
            get
            {
                string port = ConfigurationManager.AppSettings["MangaWebMySQLPort"];
                return String.IsNullOrWhiteSpace(port) ? 0 : int.Parse(port);
            }
        }

        public static string MySQLUser
        {
            get
            {
                return ConfigurationManager.AppSettings["MangaWebMySQLUser"];
            }
        }

        public static string MySQLPassword
        {
            get
            {
                return ConfigurationManager.AppSettings["MangaWebMySQLPassword"];
            }
        }

        public static string MySQLDatabase
        {
            get
            {
                return ConfigurationManager.AppSettings["MangaWebMySQLDatabase"];
            }
        }

        public static string SevenZipDllPath
        {
            get
            {
                return ConfigurationManager.AppSettings["MangaWeb7zDll"];
            }
        }

        public static string PdfinfoPath
        {
            get
            {
                return ConfigurationManager.AppSettings["MangaWebPdfinfo"];
            }
        }

        public static string MudrawPath
        {
            get
            {
                return ConfigurationManager.AppSettings["MangaWebMudraw"];
            }
        }
    }
}