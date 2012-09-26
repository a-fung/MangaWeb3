// AdminUsersSetAdminRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminUsersSetAdminRequest : JsonRequest
    {
        public int[] ids;
        public bool admin;
    }
}
