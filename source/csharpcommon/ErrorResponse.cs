// ErrorResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class ErrorResponse : JsonResponse
    {
        public int errorCode;
        public string errorMsg;
    }
}
