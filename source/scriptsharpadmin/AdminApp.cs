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
