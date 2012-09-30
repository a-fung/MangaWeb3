using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using SevenZip;

namespace afung.MangaWeb3.Server.Provider
{
    public abstract class SevenZipProvider : IMangaProvider
    {
        public SevenZipProvider()
        {
            if (!Settings.UseZip && !Settings.UseRar)
            {
                throw new InvalidOperationException("Zip/RAR formats are not configured to use.");
            }
        }

        public bool TryOpen(string path)
        {
            bool validFile = false;

            try
            {
                using (SevenZipExtractor extractor = new SevenZipExtractor(path))
                {
                    foreach (string fileName in extractor.ArchiveFileNames)
                    {
                        string extension = Utility.GetExtension(fileName).ToLowerInvariant();

                        if (Constants.FileExtensionsInArchive.Contains(extension))
                        {
                            validFile = true;
                            break;
                        }
                    }
                }
            }
            catch (SevenZipException)
            {
            }

            return validFile;
        }


        public string[] GetContent(string path)
        {
            List<string> content = new List<string>();

            try
            {
                using (SevenZipExtractor extractor = new SevenZipExtractor(path))
                {
                    foreach (string fileName in extractor.ArchiveFileNames)
                    {
                        string extension = Utility.GetExtension(fileName).ToLowerInvariant();

                        if (Constants.FileExtensionsInArchive.Contains(extension))
                        {
                            content.Add(Utility.Remove4PlusBytesUtf8Chars(fileName));
                        }
                    }
                }

                content.Sort((f1, f2) => Utility.StrCmpLogicalW(f1, f2));
            }
            catch (SevenZipException)
            {
            }

            return content.ToArray();
        }
    }
}