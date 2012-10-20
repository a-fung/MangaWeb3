// AdminSettingsSetRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminSettingsSetRequest : JsonRequest
    {
        public bool guest;
        public bool zip;
        public bool rar;
        public bool pdf;
        public int preprocessCount;
        public int preprocessDelay;
        public int cacheLimit;
    }
}
