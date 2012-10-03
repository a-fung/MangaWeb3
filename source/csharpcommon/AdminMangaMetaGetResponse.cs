// AdminMangaMetaResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminMangaMetaGetResponse : JsonResponse
    {
        public AdminMangaMetaJson meta;
        public string[] authors;
        public string[] series;
        public string[] publishers;
    }
}
