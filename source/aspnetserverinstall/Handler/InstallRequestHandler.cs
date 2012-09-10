using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using afung.MangaWeb3.Common;
using afung.MangaWeb3.Server.Handler;

namespace afung.MangaWeb3.Server.Install.Handler
{
    public class InstallRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(InstallRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            InstallRequest request = Utility.ParseJson<InstallRequest>(jsonString);

            // Check Admin user name and password
            Regex regex = new Regex("[^a-zA-Z0-9]");
            if (String.IsNullOrEmpty(request.admin) || regex.IsMatch(request.admin) || request.password == null || request.password.Length < 8 || request.password != request.password2)
            {
                ajax.BadRequest();
                return;
            }

            // Save MySQL setting, 7z, pdfinfo and mudraw to web.config
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
            AppSettingsSection section = configuration.AppSettings;
            section.Settings.Add("MangaWebInstalled", true.ToString());

            section.Settings.Add("MangaWebMySQLServer", request.mysqlServer);
            section.Settings.Add("MangaWebMySQLPort", request.mysqlPort.ToString());
            section.Settings.Add("MangaWebMySQLUser", request.mysqlUser);
            section.Settings.Add("MangaWebMySQLPassword", request.mysqlPassword);
            section.Settings.Add("MangaWebMySQLDatabase", request.mysqlDatabase);

            section.Settings.Add("MangaWeb7zDll", request.sevenZipPath);
            section.Settings.Add("MangaWebPdfinfo", request.pdfinfoPath);
            section.Settings.Add("MangaWebMudraw", request.mudrawPath);

            configuration.Save();
            ConfigurationManager.RefreshSection("appSettings");

            // Import install.sql to MySQL

            // Create Administrator

            // Save zip, rar, pdf to Settings table


        }
    }
}