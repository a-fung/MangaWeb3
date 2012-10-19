// ErrorLogJson.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class ErrorLogJson : JsonResponse
    {
        public int id;
        public int time;
        public string type;
        public string source;
        public string message;
        public string stackTrace;
    }
}
