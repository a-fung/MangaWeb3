using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

        public static bool IsValidStringForDatabase(string str)
        {
            return str == Remove4PlusBytesUtf8Chars(str);
        }

        public static string Remove4PlusBytesUtf8Chars(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte questionMark = Encoding.UTF8.GetBytes("?")[0];

            int i = 0;
            while (i < bytes.Length)
            {
                byte code = bytes[i];

                if (code >= 0 && code < 128)
                {
                    // valid one byte char
                    i++;
                    continue;
                }
                else
                {
                    int extraBytes = 0;
                    if (code >= 192 && code < 224)
                    {
                        extraBytes = 1;
                    }
                    else if (code >= 224 && code < 240)
                    {
                        extraBytes = 2;
                    }
                    else if (code >= 240 && code < 248)
                    {
                        extraBytes = 3;
                    }
                    else if (code >= 248 && code < 252)
                    {
                        extraBytes = 4;
                    }
                    else if (code >= 252 && code < 254)
                    {
                        extraBytes = 5;
                    }

                    if (extraBytes > 0 && extraBytes < 3 && i + extraBytes < bytes.Length)
                    {
                        bool valid = true;
                        int j = 1;

                        while (j <= extraBytes)
                        {
                            byte code2 = bytes[i + j];
                            if (code2 >= 128 && code2 < 192)
                            {
                            }
                            else
                            {
                                valid = false;
                                break;
                            }
                            j++;
                        }

                        if (valid)
                        {
                            i += 1 + extraBytes;
                            continue;
                        }
                    }
                    else if (i + extraBytes >= bytes.Length)
                    {
                        extraBytes = bytes.Length - i - 1;
                    }

                    // invalid byte or longer than MySQL is accepting
                    byte[] newBytes = new byte[bytes.Length - extraBytes];
                    Array.Copy(bytes, 0, newBytes, 0, i);
                    newBytes[i] = questionMark;
                    Array.Copy(bytes, i + 1 + extraBytes, newBytes, i + 1, bytes.Length - i - 1 - extraBytes);
                    bytes = newBytes;
                    i++;
                    continue;
                }
            }

            return Encoding.UTF8.GetString(bytes);
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

        public static string GetExtension(string path)
        {
            int index;
            if (path == null)
            {
                return null;
            }
            if ((index = path.LastIndexOf(".")) == -1)
            {
                return string.Empty;
            }
            else
            {
                return path.Substring(index);
            }
        }

        public static int ToUnixTimeStamp(DateTime date)
        {
            DateTime timeStampZero = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date - timeStampZero;
            return (int)Math.Round(diff.TotalSeconds);
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int StrCmpLogicalW(string x, string y);

        public static string Encode4BytesCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c >= 55296 && c <= 57343)
                {
                    // This character needs to be encoded
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else if (c == '\\')
                {
                    sb.Append("\\\\");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string DecodeEscapedUnicodeCharacters(string value)
        {
            return Regex.Replace(
                value,
                @"\\(?<Value>\\|(u[a-fA-F0-9]{4}))",
                m =>
                {
                    if (m.Groups["Value"].Value == "\\")
                    {
                        return "\\";
                    }
                    else
                    {
                        return ((char)int.Parse(m.Groups["Value"].Value.Substring(1), NumberStyles.HexNumber)).ToString();
                    }
                });
        }

        public static string JsonEncodeArchiveContent(string[] content)
        {
            string[] c = new string[content.Length];
            Array.Copy(content, c, c.Length);

            for (int i = 0; i < c.Length; i++)
            {
                c[i] = Encode4BytesCharacters(c[i]);
            }

            return JsonConvert.SerializeObject(c);
        }

        public static string[] JsonDecodeArchiveContentString(string jsonString)
        {
            string[] c = JsonConvert.DeserializeObject<string[]>(jsonString);

            for (int i = 0; i < c.Length; i++)
            {
                c[i] = DecodeEscapedUnicodeCharacters(c[i]);
            }

            return c;
        }
    }
}