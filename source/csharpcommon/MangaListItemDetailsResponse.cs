// MangaListItemDetailsResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class MangaListItemDetailsResponse : JsonResponse
    {
        public string author;
        public string series;
        public int volume;
        public int year;
        public string publisher;
        public string[] tags;
    }
}
