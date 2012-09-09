// JsonRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class JsonRequest
    {
        public string type;

        public JsonRequest()
        {
            type = this.GetType().Name;
        }
    }
}
