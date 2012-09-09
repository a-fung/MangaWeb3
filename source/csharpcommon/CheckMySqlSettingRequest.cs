// CheckMySqlSettingRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class CheckMySqlSettingRequest : JsonRequest
    {
        public string server;
        public int port;
        public string username;
        public string password;
        public string database;
    }
}
