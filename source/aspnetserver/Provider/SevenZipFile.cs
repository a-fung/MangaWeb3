using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using SevenZip;

namespace afung.MangaWeb3.Server.Provider
{
    public class SevenZipFile : IDisposable
    {
        private static readonly object lockObject = new object();

        private ExtractorUseCountPair extractor;

        public SevenZipExtractor Extractor
        {
            get
            {
                return extractor == null ? null : extractor.Extractor;
            }
        }

        private static Dictionary<string, ExtractorUseCountPair> extractors = new Dictionary<string, ExtractorUseCountPair>();

        public SevenZipFile(string path)
        {
            if (!Settings.UseZip && !Settings.UseRar)
            {
                throw new InvalidOperationException("Zip/RAR formats are not configured to use.");
            }

            lock (lockObject)
            {
                if (extractors.TryGetValue(path, out extractor))
                {
                    extractor.Count++;
                }
                else
                {
                    extractor = new ExtractorUseCountPair(path);
                    extractors[path] = extractor;
                }
            }
        }

        public void Dispose()
        {
            lock (lockObject)
            {
                if (--extractor.Count == 0)
                {
                    Thread newThread = new Thread(TryDisposeExtractor);
                    newThread.Priority = ThreadPriority.BelowNormal;
                    newThread.Start(extractor.Path);
                }
            }
        }

        private static void TryDisposeExtractor(object data)
        {
            Thread.Sleep(60000);

            string path = (string)data;
            ExtractorUseCountPair extractor;

            if (extractors.TryGetValue(path, out extractor) && extractor.Count == 0 && !extractor.Disposed && DateTime.Now - extractor.LastUsedTime >= TimeSpan.FromSeconds(60))
            {
                lock (lockObject)
                {
                    if (extractor.Count == 0 && !extractor.Disposed)
                    {
                        extractor.Dispose();
                        extractors.Remove(path);
                    }

                    extractor = null;
                }
            }
        }

        private class ExtractorUseCountPair : IDisposable
        {
            public SevenZipExtractor Extractor
            {
                get;
                private set;
            }

            public int Count
            {
                get
                {
                    return count;
                }
                set
                {
                    count = value;
                    LastUsedTime = DateTime.UtcNow;
                }
            }

            public DateTime LastUsedTime
            {
                get;
                private set;
            }

            public string Path
            {
                get;
                private set;
            }

            public bool Disposed
            {
                get;
                private set;
            }

            private int count;

            public ExtractorUseCountPair(string path)
            {
                Extractor = new SevenZipExtractor(Path = path);
                Count = 1;
                Disposed = false;
            }

            public void Dispose()
            {
                Extractor.Dispose();
                Disposed = true;
            }
        }
    }
}