// AdminModule.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Client.Module;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Client.Admin.Module
{
    public class AdminModule : ModuleBase
    {
        public AdminModule()
            : base("admin", "admin-module")
        {
        }

        protected override void Initialize()
        {
            LoginModal.GetUserName(LoginSuccess, LoginFailure, true);
        }

        private void LoginSuccess(LoginResponse userInfo)
        {
            if (userInfo.admin)
            {

            }
            else
            {
                LoginFailure(new Exception(Strings.Get("NoAdminRight")));
            }
        }

        private void LoginFailure(Exception error)
        {
            ErrorModal.ShowError(String.Format(Strings.Get("LoginFailed"), error));
        }
    }
}
