// InstallRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class InstallRequest : JsonRequest
    {
        public string mysqlServer;
        public int mysqlPort;
        public string mysqlUser;
        public string mysqlPassword;
        public string mysqlDatabase;

        public string sevenZipPath;

        public bool zip;
        public bool rar;
        public bool pdf;

        public string admin;
        public string password;
        public string password2;
    }
}
