// AdminSettingsGetResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminSettingsGetResponse : JsonResponse
    {
        public bool guest;
        public bool zip;
        public bool rar;
        public bool pdf;
    }
}
