// AdminCollectionsUsersDeleteRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminCollectionsUsersDeleteRequest : JsonRequest
    {
        public int t;
        public int id;
        public int[] ids;
    }
}
