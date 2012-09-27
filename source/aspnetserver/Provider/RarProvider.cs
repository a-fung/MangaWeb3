using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server.Provider
{
    public class RarProvider : SevenZipProvider
    {
        public const string Extension = ".rar";

        public RarProvider()
        {
            if (!Settings.UseRar)
            {
                throw new InvalidOperationException("RAR format is not configured to use.");
            }
        }
    }
}