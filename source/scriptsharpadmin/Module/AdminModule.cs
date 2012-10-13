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
        private static AdminModule _instance = null;
        public static AdminModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AdminModule();
                }

                return _instance;
            }
        }

        private AdminModule()
            : base("admin", "admin-module")
        {
        }

        protected override void OnShow()
        {
            LoginModal.GetUserName(LoginSuccess, LoginFailure, true);
        }

        private void LoginSuccess(LoginResponse userInfo)
        {
            if (userInfo.admin)
            {
                AdminCollectionsModule.Instance.Show(null);
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
