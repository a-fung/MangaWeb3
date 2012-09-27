using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server.Provider
{
    public class ZipProvider : SevenZipProvider
    {
        public const string Extension = ".zip";

        public ZipProvider()
        {
            if (!Settings.UseZip)
            {
                throw new InvalidOperationException("Zip format is not configured to use.");
            }
        }
    }
}