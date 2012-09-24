// AdminCollectionsSetPublicRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminCollectionsSetPublicRequest : JsonRequest
    {
        public int[] ids;
        public bool public_;
    }
}
