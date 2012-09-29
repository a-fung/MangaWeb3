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
                    string[] fileNames = extractor.ArchiveFileNames.ToArray();

                    foreach (string fileName in fileNames)
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
    }
}