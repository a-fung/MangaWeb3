using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server
{
    public class Settings
    {
        private static Dictionary<string, string> settings = null;

        private static Dictionary<string, string> GetSettings()
        {
            if (settings == null)
            {
                settings = new Dictionary<string, string>();
                Dictionary<string, object>[] results = Database.Select("setting");
                int i = 0;
                while (i < results.Length)
                {
                    settings.Add((string)results[i]["name"], (string)results[i]["value"]);
                    i++;
                }
            }
            return settings;
        }

        private static void SetSetting(string name, string value)
        {
            GetSettings();
            settings[name] = value;

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("name", name);
            data.Add("value", value);
            Database.Replace("setting", data);
        }

        public static bool UseZip
        {
            get
            {
                return GetSettings()["use_zip"] == "true";
            }
            set
            {
                SetSetting("use_zip", value ? "true" : "false");
            }
        }

        public static bool UseRar
        {
            get
            {
                return GetSettings()["use_rar"] == "true";
            }
            set
            {
                SetSetting("use_rar", value ? "true" : "false");
            }
        }

        public static bool UsePdf
        {
            get
            {
                return GetSettings()["use_pdf"] == "true";
            }
            set
            {
                SetSetting("use_pdf", value ? "true" : "false");
            }
        }
    }
}