using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;


namespace afung.MangaWeb3.Server
{
    public class ThreadHelper
    {
        public static void Run(string methodName, params object[] parameters)
        {
            Thread newThread = null;

            switch (methodName)
            {
                case "MangaProcessFile":
                    newThread = new Thread(MangaProcessFile);
                    break;
                case "MangaCacheLimit":
                    newThread = new Thread(MangaCacheLimit);
                    break;
                case "MangaPreprocessFiles":
                    newThread = new Thread(MangaPreprocessFiles);
                    break;
                case "MangaPreprocessParts":
                    newThread = new Thread(MangaPreprocessParts);
                    break;
                default:
                    return;
            }

            if (newThread != null)
            {
                newThread.Priority = ThreadPriority.BelowNormal;
                newThread.Start(parameters);
            }
        }

        private static void MangaProcessFile(object data)
        {
            object[] parameters = (object[])data;
            int id = (int)parameters[0];
            Manga manga = Manga.GetById(id);

            if (manga != null)
            {
                manga.ProcessFile((string)parameters[1], (int)parameters[2], (int)parameters[3], (int)parameters[4], (string)parameters[5], (string)parameters[6]);
            }
        }

        private static void MangaCacheLimit(object data)
        {
            Manga.CacheLimit();
        }

        private static void MangaPreprocessFiles(object data)
        {
            object[] parameters = (object[])data;
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

        private static void MangaPreprocessParts(object data)
        {
            object[] parameters = (object[])data;
            int id = (int)parameters[0];
            Manga manga = Manga.GetById(id);

            if (manga != null)
            {
                manga.GetPage((int)parameters[1], (int)parameters[2], (int)parameters[3], 1);
                manga.GetPage((int)parameters[1], (int)parameters[2], (int)parameters[3], 2);
            }
        }
    }
}