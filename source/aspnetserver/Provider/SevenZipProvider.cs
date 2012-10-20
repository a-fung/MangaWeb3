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
                using (SevenZipFile sevenZipFile = new SevenZipFile(path))
                {
                    lock (sevenZipFile.Extractor)
                    {
                        foreach (string fileName in sevenZipFile.Extractor.ArchiveFileNames)
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
                using (SevenZipFile sevenZipFile = new SevenZipFile(path))
                {
                    lock (sevenZipFile.Extractor)
                    {
                        foreach (string fileName in sevenZipFile.Extractor.ArchiveFileNames)
                        {
                            string extension = Utility.GetExtension(fileName).ToLowerInvariant();

                            if (Constants.FileExtensionsInArchive.Contains(extension))
                            {
                                content.Add(fileName);
                            }
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
                using (SevenZipFile sevenZipFile = new SevenZipFile(path))
                {
                    lock (sevenZipFile.Extractor)
                    {
                        outputPath = outputPath + Utility.GetExtension(content).ToLowerInvariant();
                        using (FileStream outputFile = File.Open(outputPath, FileMode.Create))
                        {
                            try
                            {
                                sevenZipFile.Extractor.ExtractFile(content, outputFile);
                            }
                            catch (SevenZipException sevenZipException)
                            {
                                throw new MangaContentMismatchException(path, sevenZipException);
                            }
                        }
                    }
                }
            }
            catch (SevenZipException sevenZipException)
            {
                throw new MangaWrongFormatException(path, sevenZipException);
            }

            return outputPath;
        }
    }
}