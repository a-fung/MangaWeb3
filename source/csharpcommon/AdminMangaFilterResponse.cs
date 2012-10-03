// AdminMangaFilterResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminMangaFilterResponse : JsonResponse
    {
        public string[] collections;
        public string[] tags;
        public string[] authors;
    }
}
