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
                            content.Add(fileName);
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

        public string OutputFile(string path, string content, string outputPath)
        {
            try
            {
                using (SevenZipExtractor extractor = new SevenZipExtractor(path))
                {
                    outputPath = outputPath + Utility.GetExtension(content).ToLowerInvariant();
                    using (FileStream outputFile = File.Open(outputPath, FileMode.Create))
                    {
                        try
                        {
                            extractor.ExtractFile(content, outputFile);
                        }
                        catch (SevenZipException sevenZipException)
                        {
                            InvalidOperationException exception = new InvalidOperationException("Read Archive file error", sevenZipException);
                            exception.Data["manga_status"] = 3;
                            throw exception;
                        }
                    }
                }
            }
            catch (SevenZipException sevenZipException)
            {
                InvalidOperationException exception = new InvalidOperationException("Read Archive file error", sevenZipException);
                exception.Data["manga_status"] = System.IO.File.Exists(path) ? 2 : 1;
                throw exception;
            }

            return outputPath;
        }
    }
}