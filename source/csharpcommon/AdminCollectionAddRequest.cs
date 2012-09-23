// AdminCollectionAddRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminCollectionAddRequest : JsonRequest
    {
        public string name;
        public string path;
        public bool public_;
        public bool autoadd;
    }
}
