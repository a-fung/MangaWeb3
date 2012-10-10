// SearchModuleResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class SearchModuleResponse : JsonResponse
    {
        public string[] authors;
        public string[] series;
        public string[] publishers;
    }
}
