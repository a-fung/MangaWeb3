// AdminApp.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Admin.Module;

namespace afung.MangaWeb3.Client.Admin
{
    public class AdminApp : Application
    {
        protected override void StartStage2()
        {
            Template.Templates[Template.Templates.Length] = "admin";
            Template.TemplateIds["admin"] = new string[] {
                "admin-manga-edit-path-modal",
                "admin-finder-modal",
                "admin-manga-add-modal",
                "admin-user-collections-trow",
                "admin-user-collections-modal",
                "admin-collection-users-trow",
                "admin-collection-users-modal",
                "admin-user-add-modal",
                "admin-collection-editname-modal",
                "admin-collection-add-modal",
                "admin-settings-modal",
                "admin-users-trow",
                "admin-users-module",
                "admin-mangas-trow",
                "admin-mangas-module",
                "admin-collections-trow",
                "admin-collections-module",
                "admin-module"
            };

            base.StartStage2();
        }

        protected override void LoadFirstModule()
        {
            new AdminModule();
        }
    }
}
