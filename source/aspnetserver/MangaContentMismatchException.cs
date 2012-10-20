using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server
{
    public class MangaContentMismatchException : ApplicationException
    {
        public MangaContentMismatchException(string file)
            : this(file, null)
        {
        }

        public MangaContentMismatchException(string file, Exception innerException)
            : base("The file (" + file + ") has content not match that in the database", innerException)
        {
        }
    }
}