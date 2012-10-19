using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using afung.MangaWeb3.Server.Provider;

namespace afung.MangaWeb3.Server
{
    public class ThreadHelper
    {
        private static object autoAddLock = new object();

        public static void Run(string methodName, params object[] parameters)
        {
            Thread newThread = new Thread(InnerRun);
            newThread.Priority = ThreadPriority.BelowNormal;
            newThread.Start(new object[] { methodName, parameters });
        }

        public static void InnerRun(object data)
        {
            try
            {
                object[] parameters = (object[])data;
                string methodName = (string)parameters[0];
                parameters = (object[])parameters[1];

                switch (methodName)
                {
                    case "MangaProcessFile":
                        MangaProcessFile(parameters);
                        break;
                    case "MangaCacheLimit":
                        MangaCacheLimit(parameters);
                        break;
                    case "MangaPreprocessFiles":
                        MangaPreprocessFiles(parameters);
                        break;
                    case "MangaPreprocessParts":
                        MangaPreprocessParts(parameters);
                        break;
                    case "ProcessAutoAddStage1":
                        ProcessAutoAddStage1(parameters);
                        break;
                    case "ProcessAutoAddStage2":
                        ProcessAutoAddStage2(parameters);
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                Utility.TryLogError(ex);
            }
        }

        private static void MangaProcessFile(object[] parameters)
        {
            int id = (int)parameters[0];
            Manga manga = Manga.GetById(id);

            if (manga != null)
            {
                manga.ProcessFile((string)parameters[1], (int)parameters[2], (int)parameters[3], (int)parameters[4], (string)parameters[5], (string)parameters[6]);
            }
        }

        private static void MangaCacheLimit(object[] parameters)
        {
            Manga.CacheLimit();
        }

        private static void MangaPreprocessFiles(object[] parameters)
        {
            int id = (int)parameters[0];
            Manga manga = Manga.GetById(id);

            if (manga != null)
            {
                int page = (int)parameters[1];

                for (int i = 1; i <= 5; i++)
                {
                    if (page + i >= 0 && page + i < manga.NumberOfPages)
                    {
                        manga.GetPage(page + i, (int)parameters[2], (int)parameters[3], 0);
                    }

                    if (page - i >= 0 && page - i < manga.NumberOfPages)
                    {
                        manga.GetPage(page - i, (int)parameters[2], (int)parameters[3], 0);
                    }
                }
            }
        }

        private static void MangaPreprocessParts(object[] parameters)
        {
            int id = (int)parameters[0];
            Manga manga = Manga.GetById(id);

            if (manga != null)
            {
                manga.GetPage((int)parameters[1], (int)parameters[2], (int)parameters[3], 1);
                manga.GetPage((int)parameters[1], (int)parameters[2], (int)parameters[3], 2);
            }
        }

        private static void ProcessAutoAddStage1(object[] parameters)
        {
            if (Utility.ToUnixTimeStamp(DateTime.UtcNow) - Settings.LastAutoAddProcessTime < 300)
            {
                return;
            }

            lock (autoAddLock)
            {
                if (Utility.ToUnixTimeStamp(DateTime.UtcNow) - Settings.LastAutoAddProcessTime < 300)
                {
                    return;
                }

                Settings.LastAutoAddProcessTime = Utility.ToUnixTimeStamp(DateTime.UtcNow);
            }

            List<object[]> files = new List<object[]>();
            Collection[] collections = Collection.GetAutoAdd();

            foreach (Collection collection in collections)
            {
                Dictionary<string, object>[] resultSet = Database.Select("manga", "`cid`=" + Database.Quote(collection.Id.ToString()), null, null, "`path`");
                List<FileInfo> filesUnderCollection = new List<FileInfo>();
                DirectoryInfo collectionDirectory = new DirectoryInfo(collection.Path);
                FileInfo[] empty = { };

                filesUnderCollection.AddRange(Settings.UseZip ? collectionDirectory.GetFiles("*" + ZipProvider.Extension, SearchOption.AllDirectories) : empty);
                filesUnderCollection.AddRange(Settings.UseRar ? collectionDirectory.GetFiles("*" + RarProvider.Extension, SearchOption.AllDirectories) : empty);
                filesUnderCollection.AddRange(Settings.UsePdf ? collectionDirectory.GetFiles("*" + PdfProvider.Extension, SearchOption.AllDirectories) : empty);

                foreach (FileInfo fileUnderCollection in filesUnderCollection)
                {
                    if (Settings.LastAutoAddProcessTime - Utility.ToUnixTimeStamp(fileUnderCollection.LastWriteTimeUtc) <= 60)
                    {
                        continue;
                    }

                    bool hit = false;
                    foreach (Dictionary<string, object> result in resultSet)
                    {
                        if (fileUnderCollection.FullName.Equals(Convert.ToString(result["path"]), StringComparison.InvariantCultureIgnoreCase))
                        {
                            hit = true;
                            break;
                        }
                    }

                    if (!hit)
                    {
                        files.Add(new object[] { fileUnderCollection.FullName, collection.Id });
                    }
                }
            }

            ThreadHelper.Run("ProcessAutoAddStage2", files.ToArray(), 0);
        }

        private static void ProcessAutoAddStage2(object[] parameters)
        {
            Settings.LastAutoAddProcessTime = Utility.ToUnixTimeStamp(DateTime.UtcNow);
            object[][] files = (object[][])parameters[0];
            int index = (int)parameters[1];

            if (index >= files.Length)
            {
                return;
            }

            try
            {
                Collection collection = Collection.GetById((int)files[index][1]);
                string path = (string)files[index][0];

                if (collection != null && (path = Manga.CheckMangaPath(path)) != null && Utility.IsValidStringForDatabase(path))
                {
                    if (path.IndexOf(collection.Path, StringComparison.InvariantCultureIgnoreCase) == 0 && Manga.CheckMangaType(path) != -1)
                    {
                        Manga.CreateNewManga(collection, path).Save();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.TryLogError(ex);
            }

            ThreadHelper.Run("ProcessAutoAddStage2", files, index + 1);
        }
    }
}