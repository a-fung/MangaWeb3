// Application.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Install.Module;


namespace afung.MangaWeb3.Client.Install
{
    /// <summary>
    /// Class InstallApp
    /// </summary>
    public class InstallApp : Application
    {
        protected override void StartStage2()
        {
            Template.Templates[Template.Templates.Length] = "install";
            Template.TemplateIds["install"] = new string[] {
                "install-finish-module",
                "installing-modal",
                "install-pdfdraw-error",
                "install-pdfinfo-error",
                "install-rar-error",
                "install-zip-error",
                "install-sevenzip-error",
                "install-gd-error",
                "install-mysql-connect-error",
                "install-mysql-error",
                "install-module"
            };

            Request.Endpoint = "InstallAjax";

            base.StartStage2();
        }

        protected override void LoadFirstModule()
        {
            InstallModule.Instance.Show(null);
        }
    }
}
