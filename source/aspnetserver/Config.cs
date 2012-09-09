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
    }
}