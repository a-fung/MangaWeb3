// AdminCollectionsUsersAccessRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminCollectionsUsersAccessRequest : JsonRequest
    {
        public int t;
        public int id;
        public int[] ids;
        public bool access;
    }
}
