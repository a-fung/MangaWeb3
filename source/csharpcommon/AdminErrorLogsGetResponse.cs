// AdminErrorLogsGetResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminErrorLogsGetResponse : JsonResponse
    {
        public int numberOfLogs;
        public ErrorLogJson[] logs;
    }
}
