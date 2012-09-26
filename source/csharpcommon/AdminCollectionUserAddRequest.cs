// AdminCollectionUserAddRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminCollectionUserAddRequest : JsonRequest
    {
        public string collectionName;
        public string username;
        public bool access;
    }
}
