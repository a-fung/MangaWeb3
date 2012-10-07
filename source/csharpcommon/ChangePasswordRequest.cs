// ChangePasswordRequest.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Common
{
    public class ChangePasswordRequest : JsonRequest
    {
        public string currentPassword;
        public string newPassword;
        public string newPassword2;
    }
}
