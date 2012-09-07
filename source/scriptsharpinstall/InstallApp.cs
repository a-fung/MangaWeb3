// Application.cs
//

using System;
using System.Collections.Generic;

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
                "install-mudraw-error",
                "install-pdfdraw-error",
                "install-pdfinfo-error",
                "install-pdfinfoexe-error",
                "install-rar-error",
                "install-zip-error",
                "install-sevenzip-error",
                "install-gd-error",
                "install-mysql-connect-error",
                "install-mysql-error",
                "install-module"
            };
            base.StartStage2();
        }
    }
}
