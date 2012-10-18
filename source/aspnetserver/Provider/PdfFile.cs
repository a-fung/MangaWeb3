using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using PDFLibNet;

namespace afung.MangaWeb3.Server.Provider
{
    public class PdfFile : IDisposable
    {
        private static readonly object lockObject = new object();

        private WrapperUseCountPair wrapper;

        public PDFWrapper Wrapper
        {
            get
            {
                return wrapper == null ? null : wrapper.Wrapper;
            }
        }

        private static Dictionary<string, WrapperUseCountPair> wrappers = new Dictionary<string, WrapperUseCountPair>();

        public PdfFile(string path)
        {
            if (!Settings.UsePdf)
            {
                throw new InvalidOperationException("PDF formats are not configured to use.");
            }

            lock (lockObject)
            {
                if (wrappers.TryGetValue(path, out wrapper))
                {
                    wrapper.Count++;
                }
                else
                {
                    wrapper = new WrapperUseCountPair(path);
                    wrappers[path] = wrapper;
                }
            }
        }

        public void Dispose()
        {
            lock (lockObject)
            {
                if (--wrapper.Count == 0)
                {
                    Thread newThread = new Thread(TryDisposeWrapper);
                    newThread.Priority = ThreadPriority.BelowNormal;
                    newThread.Start(wrapper.Path);
                }
            }
        }

        private static void TryDisposeWrapper(object data)
        {
            Thread.Sleep(60000);

            string path = (string)data;
            WrapperUseCountPair wrapper;

            if (wrappers.TryGetValue(path, out wrapper) && wrapper.Count == 0 && !wrapper.Disposed && DateTime.Now - wrapper.LastUsedTime >= TimeSpan.FromSeconds(60))
            {
                lock (lockObject)
                {
                    if (wrapper.Count == 0 && !wrapper.Disposed)
                    {
                        wrapper.Dispose();
                        wrappers.Remove(path);
                    }

                    wrapper = null;
                }
            }
        }

        private class WrapperUseCountPair : IDisposable
        {
            public PDFWrapper Wrapper
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
            private Stream stream;

            public WrapperUseCountPair(string path)
            {
                Wrapper = new PDFWrapper();
                if (Wrapper.SupportsMuPDF)
                {
                    Wrapper.UseMuPDF = true;
                }

                Wrapper.LoadPDF(stream = new FileStream(Path = path, FileMode.Open));
                Count = 1;
                Disposed = false;
            }

            public void Dispose()
            {
                stream.Close();
                Wrapper.Dispose();
                Disposed = true;
            }
        }
    }
}