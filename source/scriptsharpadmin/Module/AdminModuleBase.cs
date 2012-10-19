// AdminModuleBase.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Admin.Modal;
using afung.MangaWeb3.Client.Module;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Module
{
    public abstract class AdminModuleBase : ModuleBase
    {
        protected AdminModuleBase(string templateId)
            : base("admin", templateId)
        {
            jQuery.Select(".nav-admin-collections", attachedObject).Click(NavCollectionsClicked);
            jQuery.Select(".nav-admin-mangas", attachedObject).Click(NavMangasClicked);
            jQuery.Select(".nav-admin-users", attachedObject).Click(NavUsersClicked);
            jQuery.Select(".nav-admin-settings", attachedObject).Click(NavSettingsClicked);
            jQuery.Select(".nav-admin-errorlogs", attachedObject).Click(NavErrorLogsClicked);
            jQuery.Select(".nav-admin-logout", attachedObject).Click(NavLogoutClicked);
        }

        private void NavCollectionsClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (this.GetType() != typeof(AdminCollectionsModule))
            {
                AdminCollectionsModule.Instance.Show(null);
            }
        }

        private void NavMangasClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (this.GetType() != typeof(AdminMangasModule))
            {
                AdminMangasModule.Instance.Show(null);
            }
        }

        private void NavUsersClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (this.GetType() != typeof(AdminUsersModule))
            {
                AdminUsersModule.Instance.Show(null);
            }
        }

        private void NavErrorLogsClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (this.GetType() != typeof(AdminErrorLogsModule))
            {
                AdminErrorLogsModule.Instance.Show(null);
            }
        }

        private void NavSettingsClicked(jQueryEvent e)
        {
            e.PreventDefault();
            AdminSettingsModal.ShowDialog();
        }

        private void NavLogoutClicked(jQueryEvent e)
        {
            e.PreventDefault();

            LoginRequest request = new LoginRequest();
            request.password = "logout";

            Request.Send(request, LogoutSuccessful, LogoutSuccessful);
        }

        [AlternateSignature]
        private extern void LogoutSuccessful(Exception error);
        [AlternateSignature]
        private extern void LogoutSuccessful(JsonResponse response);
        private void LogoutSuccessful()
        {
            // redirect to index.html
            Window.Location.Href = "index.html";
        }
    }
}
