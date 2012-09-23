using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace afung.MangaWeb3.Server
{
    public class Utility
    {
        private static System.Security.Cryptography.MD5CryptoServiceProvider crypto = new System.Security.Cryptography.MD5CryptoServiceProvider();

        public static T ParseJson<T>(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static string Md5(string input)
        {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = crypto.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

        public static string GetFullPath(string path)
        {
            try
            {
                string combined = Path.Combine(AjaxBase.DirectoryPath, path);
                string root = Path.GetPathRoot(combined);

                if (root == "" || root == "/" || root == "\\")
                {
                    combined = Path.GetPathRoot(AjaxBase.DirectoryPath) + combined;
                }

                return new DirectoryInfo(combined).FullName;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}