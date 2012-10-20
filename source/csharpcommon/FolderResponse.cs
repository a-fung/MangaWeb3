// FolderResponse.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class FolderResponse : JsonResponse
    {
        public int status;
        public FolderJson[] folders;
    }
}
