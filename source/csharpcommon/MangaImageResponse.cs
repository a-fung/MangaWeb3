// MangaImageResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class MangaImageResponse : JsonResponse
    {
        public int status;
        public string url;
        public int[] dimensions;
    }
}
