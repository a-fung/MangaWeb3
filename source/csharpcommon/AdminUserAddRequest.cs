// AdminUserAddRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminUserAddRequest : JsonRequest
    {
        public string username;
        public string password;
        public string password2;
        public bool admin;
    }
}
