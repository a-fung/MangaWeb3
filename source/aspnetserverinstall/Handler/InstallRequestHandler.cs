using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
            foreach (string key in new string[] { "MangaWebInstalled", "MangaWebMySQLServer", "MangaWebMySQLPort", "MangaWebMySQLUser", "MangaWebMySQLPassword", "MangaWebMySQLDatabase", "MangaWeb7zDll", "MangaWebPdfinfo", "MangaWebMudraw" })
            {
                if (section.Settings.AllKeys.Contains(key))
                {
                    section.Settings.Remove(key);
                }
            }

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
            NameValueCollection settings = new NameValueCollection();
            foreach (string key in section.Settings.AllKeys)
            {
                settings[key] = section.Settings[key].Value;
            }

            ConfigurationManager.RefreshSection("appSettings");
            Config.Refresh(settings);

            // Import install.sql to MySQL
            using (StreamReader sqlFile = new StreamReader(Path.Combine(AjaxBase.DirectoryPath, "install.sql"))) Database.ExecuteSql(sqlFile.ReadToEnd());

            // Create Administrator
            User.CreateNewUser(request.admin, request.password, true).Save();

            // Save zip, rar, pdf to Settings table
            Settings.UseZip = request.zip;
            Settings.UseRar = request.rar;
            Settings.UsePdf = request.pdf;

            // Delete Install files
            string[] filesToDelete = 
            {
                "install.html",
                "install.sql",
                "InstallAjax.aspx",
                @"bin\afung.MangaWeb3.Server.Install.*",
                @"js\afung.MangaWeb3.Client.Install.*",
                @"template\install.html",
            };

            StringBuilder argumentBuilder = new StringBuilder();
            argumentBuilder.Append("/C ping 1.1.1.1 -n 1 -w 3000 > Nul");
            foreach (string fileToDelete in filesToDelete)
            {
                argumentBuilder.AppendFormat(" & del \"{0}\"", Path.Combine(AjaxBase.DirectoryPath, fileToDelete));
            }

            string argument = argumentBuilder.ToString();
            ThreadStart runDeleteFilesCmd = delegate()
            {
                string output;
                int exitCode;
                ProcessLauncher.Run("cmd.exe", argument, out output, out exitCode);
            };

            new Thread(runDeleteFilesCmd).Start();

            InstallResponse response = new InstallResponse();
            response.installsuccessful = true;

            ajax.ReturnJson(response);
        }
    }
}