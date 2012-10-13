﻿using System;
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
            switch (methodName)
            {
                case "MangaProcessFile":
                    new Thread(MangaProcessFile).Start(parameters);
                    break;
                case "MangaCacheLimit":
                    new Thread(MangaCacheLimit).Start(parameters);
                    break;
                case "MangaPreprocessFiles":
                    new Thread(MangaPreprocessFiles).Start(parameters);
                    break;
                case "MangaPreprocessParts":
                    new Thread(MangaPreprocessParts).Start(parameters);
                    break;
                default:
                    return;
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