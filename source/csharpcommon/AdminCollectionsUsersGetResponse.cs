// AdminCollectionsUsersGetResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminCollectionsUsersGetResponse : JsonResponse
    {
        public string name;
        public CollectionUserJson[] data;
        public string[] names;
    }
}
