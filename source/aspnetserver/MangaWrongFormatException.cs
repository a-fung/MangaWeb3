using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server
{
    public class MangaWrongFormatException : ApplicationException
    {
        public MangaWrongFormatException(string file)
            : this(file, null)
        {
        }

        public MangaWrongFormatException(string file, Exception innerException)
            : base("The file (" + file + ") is not in the right format", innerException)
        {
        }
    }
}