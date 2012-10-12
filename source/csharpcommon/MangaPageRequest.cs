// MangaPageRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class MangaPageRequest : JsonRequest
    {
        public int id;
        public int page;
        public int width;
        public int height;
        public int part;
        public bool dimensions;
    }
}
