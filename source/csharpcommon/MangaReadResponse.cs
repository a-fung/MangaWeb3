// MangaReadResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class MangaReadResponse : JsonResponse
    {
        public int id;
        public int pages;
        public int nextId;
    }
}
