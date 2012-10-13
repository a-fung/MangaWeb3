using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SevenZip;

namespace afung.MangaWeb3.Server
{
    public class Settings
    {
        private static object lockObject = new object();

        private static Dictionary<string, string> settings = null;

        private static Dictionary<string, string> GetSettings()
        {
            if (settings == null)
            {
                lock (lockObject)
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

        public static bool AllowGuest
        {
            get
            {
                return GetSettings()["allow_guest"] != "false";
            }
            set
            {
                SetSetting("allow_guest", value ? "true" : "false");
            }
        }

        public static bool UseZip
        {
            get
            {
                return GetSettings()["use_zip"] == "true" && SevenZipConfigOk;
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
                return GetSettings()["use_rar"] == "true" && SevenZipConfigOk;
            }
            set
            {
                SetSetting("use_rar", value ? "true" : "false");
            }
        }

        private static bool? _usePdf = null;

        public static bool UsePdf
        {
            get
            {
                if (_usePdf == null)
                {
                    lock (lockObject)
                    {
                        if (_usePdf == null)
                        {
                            if (GetSettings()["use_pdf"] != "true")
                            {
                                return (_usePdf = false).Value;
                            }
                            else
                            {
                                try
                                {
                                    int exitCode;
                                    string output;
                                    ProcessLauncher.Run(Config.PdfinfoPath, Path.Combine(AjaxBase.DirectoryPath, "empty.pdf"), out output, out exitCode);
                                    if (exitCode != 0)
                                    {
                                        return (_usePdf = false).Value;
                                    }

                                    if (!output.Contains("Pages"))
                                    {
                                        return (_usePdf = false).Value;
                                    }

                                    ProcessLauncher.Run(Config.MudrawPath, Path.Combine(AjaxBase.DirectoryPath, "empty.pdf"), out output, out exitCode);
                                    if (exitCode != 0)
                                    {
                                        return (_usePdf = false).Value;
                                    }

                                    if (!output.Contains("nothing to do"))
                                    {
                                        return (_usePdf = false).Value;
                                    }
                                }
                                catch (Exception)
                                {
                                    return (_usePdf = false).Value;
                                }
                            }
                        }
                    }
                }

                return _usePdf.Value;
            }
            set
            {
                _usePdf = null;
                SetSetting("use_pdf", value ? "true" : "false");
            }
        }

        private static bool? _sevenZipConfigOk = null;

        private static bool SevenZipConfigOk
        {
            get
            {
                if (_sevenZipConfigOk == null)
                {
                    lock (lockObject)
                    {
                        if (_sevenZipConfigOk == null)
                        {
                            try
                            {
                                SevenZipBase.SetLibraryPath(Config.SevenZipDllPath);
                                SevenZipCompressor c = new SevenZipCompressor();
                                using (System.IO.MemoryStream m1 = new MemoryStream())
                                {
                                    using (System.IO.MemoryStream m2 = new MemoryStream())
                                    {
                                        byte[] b = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                                        m1.Write(b, 0, b.Length);
                                        c.CompressStream(m1, m2);
                                    }
                                }

                                _sevenZipConfigOk = true;
                            }
                            catch (Exception)
                            {
                                _sevenZipConfigOk = false;
                            }
                        }
                    }
                }

                return _sevenZipConfigOk.Value;
            }
        }
    }
}