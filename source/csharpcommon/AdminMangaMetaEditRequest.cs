// AdminMangaMetaEditRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminMangaMetaEditRequest : JsonRequest
    {
        public int id;
        public AdminMangaMetaJson meta;
    }
}
