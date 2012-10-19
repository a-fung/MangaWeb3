// AdminErrorLogsGetRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class AdminErrorLogsGetRequest : JsonRequest
    {
        public int page;
        public int elementsPerPage;
    }
}
