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
