// PreInstallCheckResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class PreInstallCheckResponse : JsonResponse
    {
        public bool installed;
        public bool mySql;
        public bool gd;
        public bool zip;
        public bool rar;
        public bool pdfinfo;
        public bool pdfdraw;
    }
}
