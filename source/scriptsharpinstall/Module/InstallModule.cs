// InstallModule.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Install.Modal;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Client.Module;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Install.Module
{
    public class InstallModule : ModuleBase
    {
        private bool _canEnableZip = false;
        private bool CanEnableZip
        {
            get
            {
                return _canEnableZip;
            }
            set
            {
                if (value)
                {
                    jQuery.Select("#install-zip-checkbox", attachedObject).RemoveAttr("disabled");
                }
                else
                {
                    jQuery.Select("#install-zip-checkbox", attachedObject)
                        .Attribute("disabled", "disabled")
                        .RemoveAttr("checked");
                }
                _canEnableZip = value;
            }
        }

        private bool _canEnableRar = false;
        private bool CanEnableRar
        {
            get
            {
                return _canEnableRar;
            }
            set
            {
                if (value)
                {
                    jQuery.Select("#install-rar-checkbox", attachedObject).RemoveAttr("disabled");
                }
                else
                {
                    jQuery.Select("#install-rar-checkbox", attachedObject)
                        .Attribute("disabled", "disabled")
                        .RemoveAttr("checked");
                }
                _canEnableRar = value;
            }
        }

        private bool _canEnablePdf = false;
        private bool CanEnablePdf
        {
            get
            {
                return _canEnablePdf;
            }
            set
            {
                if (value)
                {
                    jQuery.Select("#install-pdf-checkbox", attachedObject).RemoveAttr("disabled");
                }
                else
                {
                    jQuery.Select("#install-pdf-checkbox", attachedObject)
                        .Attribute("disabled", "disabled")
                        .RemoveAttr("checked");
                }
                _canEnablePdf = value;
            }
        }

        private bool AllRequiredComponentLoaded;

        private static InstallModule _instance = null;
        public static InstallModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InstallModule();
                }

                return _instance;
            }
        }

        private InstallModule()
            : base("install", "install-module")
        {
            jQuery.Select("#install-form").Hide();
            jQuery.Select("#install-language").Change(LanguageChange);
        }

        protected override void OnShow()
        {
            Request.Send(new PreInstallCheckRequest(), OnPreInstallCheckRequestFinished);
            jQuery.Select("#install-language").Value(Settings.UserLanguage);
        }

        [AlternateSignature]
        private extern void OnPreInstallCheckRequestFinished(JsonResponse response);
        private void OnPreInstallCheckRequestFinished(PreInstallCheckResponse response)
        {
            if (response.installed)
            {
                // switch to install finish module
                return;
            }

            jQuery.Select("#install-preinstall-check").Hide();
            jQuery.Select("#install-form").Show();

            jQuery.Select("#install-mysql-check").Hide();
            jQuery.Select("#install-mysql-loading").Hide();

            jQuery.Select("#install-sevenzip-check").Hide();
            jQuery.Select("#install-sevenzip-loading").Hide();

            jQuery.Select("#install-pdfinfoexe-check").Hide();
            jQuery.Select("#install-pdfinfoexe-loading").Hide();

            jQuery.Select("#install-mudraw-check").Hide();
            jQuery.Select("#install-mudraw-loading").Hide();

            jQuery.Select("#install-admin-username-check").Hide();
            jQuery.Select("#install-admin-password-check").Hide();
            jQuery.Select("#install-admin-password2-check").Hide();

            if (Environment.ServerType == ServerType.AspNet)
            {
                CanEnableZip = CanEnableRar = CanEnablePdf = false;
                AllRequiredComponentLoaded = true;

                // other components text input
                jQuery.Select("#install-sevenzip-dll").Change(OtherComponentInputChanged);
                jQuery.Select("#install-pdfinfo-exe").Change(OtherComponentInputChanged);
                jQuery.Select("#install-mudraw-exe").Change(OtherComponentInputChanged);
            }
            else
            {
                jQuery.Select("#install-sevenzip").Hide();
                jQuery.Select("#install-pdfinfoexe").Hide();
                jQuery.Select("#install-mudraw").Hide();

                CanEnableZip = response.zip;
                CanEnableRar = response.rar;
                CanEnablePdf = response.pdfinfo && response.pdfdraw;
                AllRequiredComponentLoaded = response.mySql && response.gd;

                if (!response.mySql)
                {
                    Template.Get("install", "install-mysql-error").AppendTo(jQuery.Select("#mysql-error-area"));
                }

                if (!response.gd)
                {
                    Template.Get("install", "install-gd-error").AppendTo(jQuery.Select("#gd-error-area"));
                }

                if (!response.zip)
                {
                    Template.Get("install", "install-zip-error").AppendTo(jQuery.Select("#zip-error-area"));
                }

                if (!response.rar)
                {
                    Template.Get("install", "install-rar-error").AppendTo(jQuery.Select("#rar-error-area"));
                }

                if (!response.pdfinfo)
                {
                    Template.Get("install", "install-pdfinfo-error").AppendTo(jQuery.Select("#pdfinfo-error-area"));
                }

                if (!response.pdfdraw)
                {
                    Template.Get("install", "install-pdfdraw-error").AppendTo(jQuery.Select("#mudraw-error-area"));
                }
            }

            // MySql text inputs and button
            jQuery.Select("#install-mysql-check-setting").Click(MySqlCheckSettingClicked);

            jQuery.Select("#install-mysql-server").Change(MySqlCheckSettingChanged);
            jQuery.Select("#install-mysql-port").Change(MySqlCheckSettingChanged);
            jQuery.Select("#install-mysql-username").Change(MySqlCheckSettingChanged);
            jQuery.Select("#install-mysql-password").Change(MySqlCheckSettingChanged);
            jQuery.Select("#install-mysql-database").Change(MySqlCheckSettingChanged);

            // Admin Section
            jQuery.Select("#install-admin-username").Change(AdminUserChanged);
            jQuery.Select("#install-admin-password").Change(AdminPasswordChanged);
            jQuery.Select("#install-admin-password2").Change(AdminConfirmPasswordChanged);

            // Submit button
            jQuery.Select("#install-submit-btn").Click(SubmitButtonClicked);
        }

        private bool _mySqlSettingChecking = false;
        private bool MySqlSettingChecking
        {
            get
            {
                return _mySqlSettingChecking;
            }
            set
            {
                _mySqlSettingChecking = value;
                if (value)
                {
                    jQuery.Select("#install-mysql-loading").Show();
                }
                else
                {
                    jQuery.Select("#install-mysql-loading").Hide();
                }
            }
        }

        private bool _mySqlSettingOkay = false;
        private bool MySqlSettingOkay
        {
            get
            {
                return _mySqlSettingOkay;
            }
            set
            {
                _mySqlSettingOkay = value;
                if (value)
                {
                    jQuery.Select("#install-mysql-check").Show();
                }
                else
                {
                    jQuery.Select("#install-mysql-check").Hide();
                }
            }
        }

        private void MySqlCheckSettingChanged(jQueryEvent e)
        {
            MySqlSettingOkay = false;
        }

        private void MySqlCheckSettingClicked(jQueryEvent e)
        {
            if (MySqlSettingChecking) return;

            string server = jQuery.Select("#install-mysql-server").GetValue();
            string portString = jQuery.Select("#install-mysql-port").GetValue();
            string username = jQuery.Select("#install-mysql-username").GetValue();
            string password = jQuery.Select("#install-mysql-password").GetValue();
            string database = jQuery.Select("#install-mysql-database").GetValue();

            if (String.IsNullOrEmpty(server) || String.IsNullOrEmpty(portString) || String.IsNullOrEmpty(username) || String.IsNullOrEmpty(database))
            {
                return;
            }

            int port = int.Parse(portString, 10);

            CheckMySqlSettingRequest request = new CheckMySqlSettingRequest();
            request.server = server;
            request.port = port;
            request.username = username;
            request.password = password;
            request.database = database;

            jQuery.Select("#install-mysql-connect-error").Remove();
            MySqlSettingChecking = true;
            MySqlSettingOkay = false;
            Request.Send(request, MysqlCheckSuccess, MySqlCheckFailed);
        }

        [AlternateSignature]
        private extern void MysqlCheckSuccess(JsonResponse response);
        private void MysqlCheckSuccess(CheckMySqlSettingResponse response)
        {
            MySqlSettingChecking = false;
            MySqlSettingOkay = response.pass;
            if (!response.pass)
            {
                MySqlCheckFailed();
            }
        }

        [AlternateSignature]
        private extern void MySqlCheckFailed(Exception error);
        private void MySqlCheckFailed()
        {
            MySqlSettingOkay = MySqlSettingChecking = false;
            Template.Get("install", "install-mysql-connect-error").AppendTo(jQuery.Select("#mysql-error-area"));
        }

        private bool _sevenZipSettingChecking = false;
        private bool SevenZipSettingChecking
        {
            get
            {
                return _sevenZipSettingChecking;
            }
            set
            {
                _sevenZipSettingChecking = value;
                if (value)
                {
                    jQuery.Select("#install-sevenzip-loading").Show();
                    jQuery.Select("#install-sevenzip-dll").Attribute("disabled", "disabled");
                }
                else
                {
                    jQuery.Select("#install-sevenzip-loading").Hide();
                    jQuery.Select("#install-sevenzip-dll").RemoveAttr("disabled");
                }
            }
        }

        private bool _sevenZipSettingOkay = false;
        private bool SevenZipSettingOkay
        {
            get
            {
                return _sevenZipSettingOkay;
            }
            set
            {
                CanEnableZip = CanEnableRar = _sevenZipSettingOkay = value;
                if (value)
                {
                    jQuery.Select("#install-sevenzip-check").Show();
                }
                else
                {
                    jQuery.Select("#install-sevenzip-check").Hide();
                }
            }
        }

        private bool _pdfinfoSettingChecking = false;
        private bool PdfinfoSettingChecking
        {
            get
            {
                return _pdfinfoSettingChecking;
            }
            set
            {
                _pdfinfoSettingChecking = value;
                if (value)
                {
                    jQuery.Select("#install-pdfinfoexe-loading").Show();
                    jQuery.Select("#install-pdfinfo-exe").Attribute("disabled", "disabled");
                }
                else
                {
                    jQuery.Select("#install-pdfinfoexe-loading").Hide();
                    jQuery.Select("#install-pdfinfo-exe").RemoveAttr("disabled");
                }
            }
        }

        private bool _pdfinfoSettingOkay = false;
        private bool PdfinfoSettingOkay
        {
            get
            {
                return _pdfinfoSettingOkay;
            }
            set
            {
                _pdfinfoSettingOkay = value;
                CanEnablePdf = value && MudrawSettingOkay;
                if (value)
                {
                    jQuery.Select("#install-pdfinfoexe-check").Show();
                }
                else
                {
                    jQuery.Select("#install-pdfinfoexe-check").Hide();
                }
            }
        }

        private bool _mudrawSettingChecking = false;
        private bool MudrawSettingChecking
        {
            get
            {
                return _mudrawSettingChecking;
            }
            set
            {
                _mudrawSettingChecking = value;
                if (value)
                {
                    jQuery.Select("#install-mudraw-loading").Show();
                    jQuery.Select("#install-mudraw-exe").Attribute("disabled", "disabled");
                }
                else
                {
                    jQuery.Select("#install-mudraw-loading").Hide();
                    jQuery.Select("#install-mudraw-exe").RemoveAttr("disabled");
                }
            }
        }

        private bool _mudrawSettingOkay = false;
        private bool MudrawSettingOkay
        {
            get
            {
                return _mudrawSettingOkay;
            }
            set
            {
                _mudrawSettingOkay = value;
                CanEnablePdf = value && PdfinfoSettingOkay;
                if (value)
                {
                    jQuery.Select("#install-mudraw-check").Show();
                }
                else
                {
                    jQuery.Select("#install-mudraw-check").Hide();
                }
            }
        }

        private int checkingComponent;

        private void OtherComponentInputChanged(jQueryEvent e)
        {
            jQueryObject eventSource = jQuery.FromElement(e.Target);
            string inputId = eventSource.GetAttribute("id");
            bool checking = false;

            switch (inputId)
            {
                case "install-sevenzip-dll":
                    checking = SevenZipSettingChecking;
                    break;
                case "install-pdfinfo-exe":
                    checking = PdfinfoSettingChecking;
                    break;
                case "install-mudraw-exe":
                    checking = MudrawSettingChecking;
                    break;
                default:
                    return;
            }

            if (checking)
            {
                return;
            }

            string path = eventSource.GetValue();
            checking = !String.IsNullOrEmpty(path);

            CheckOtherComponentRequest request = new CheckOtherComponentRequest();
            request.path = path;

            switch (inputId)
            {
                case "install-sevenzip-dll":
                    SevenZipSettingChecking = checking;
                    SevenZipSettingOkay = false;
                    request.component = 0;
                    jQuery.Select("#install-sevenzip-error").Remove();
                    break;
                case "install-pdfinfo-exe":
                    PdfinfoSettingChecking = checking;
                    PdfinfoSettingOkay = false;
                    request.component = 1;
                    jQuery.Select("#install-pdfinfoexe-error").Remove();
                    break;
                case "install-mudraw-exe":
                    MudrawSettingChecking = checking;
                    MudrawSettingOkay = false;
                    request.component = 2;
                    jQuery.Select("#install-mudraw-error").Remove();
                    break;
                default:
                    return;
            }

            if (!checking)
            {
                return;
            }

            checkingComponent = request.component;
            Request.Send(request, OtherComponentCheckSuccess, OtherComponentCheckFailed);
        }

        [AlternateSignature]
        private extern void OtherComponentCheckSuccess(JsonResponse response);
        private void OtherComponentCheckSuccess(CheckOtherComponentResponse response)
        {
            if (response.pass)
            {
                switch (checkingComponent)
                {
                    case 0:
                        SevenZipSettingChecking = false;
                        SevenZipSettingOkay = true;
                        break;
                    case 1:
                        PdfinfoSettingChecking = false;
                        PdfinfoSettingOkay = true;
                        break;
                    case 2:
                        MudrawSettingChecking = false;
                        MudrawSettingOkay = true;
                        break;
                    default:
                        return;
                }
            }
            else
            {
                OtherComponentCheckFailed();
            }
        }

        [AlternateSignature]
        private extern void OtherComponentCheckFailed(Exception error);
        private void OtherComponentCheckFailed()
        {
            switch (checkingComponent)
            {
                case 0:
                    SevenZipSettingOkay = SevenZipSettingChecking = false;
                    Template.Get("install", "install-sevenzip-error").AppendTo(jQuery.Select("#sevenzip-error-area"));
                    break;
                case 1:
                    PdfinfoSettingOkay = PdfinfoSettingChecking = false;
                    Template.Get("install", "install-pdfinfoexe-error").AppendTo(jQuery.Select("#pdfinfo-error-area"));
                    break;
                case 2:
                    MudrawSettingOkay = MudrawSettingChecking = false;
                    Template.Get("install", "install-mudraw-error").AppendTo(jQuery.Select("#mudraw-error-area"));
                    break;
                default:
                    return;
            }
        }

        private void AdminUserChanged(jQueryEvent e)
        {
            RegularExpression regex = new RegularExpression("[^a-zA-Z0-9]");

            string username = jQuery.Select("#install-admin-username").GetValue();
            if (username == "" || regex.Test(username))
            {
                jQuery.Select("#install-admin-username-check").Hide();
            }
            else
            {
                jQuery.Select("#install-admin-username-check").Show();
            }
        }

        private void AdminPasswordChanged(jQueryEvent e)
        {
            if (jQuery.Select("#install-admin-password").GetValue().Length >= 8)
            {
                jQuery.Select("#install-admin-password-check").Show();
            }
            else
            {
                jQuery.Select("#install-admin-password-check").Hide();
            }
        }

        private void AdminConfirmPasswordChanged(jQueryEvent e)
        {
            if (jQuery.Select("#install-admin-password2").GetValue().Length >= 8 && jQuery.Select("#install-admin-password").GetValue() == jQuery.Select("#install-admin-password2").GetValue())
            {
                jQuery.Select("#install-admin-password2-check").Show();
            }
            else
            {
                jQuery.Select("#install-admin-password2-check").Hide();
            }
        }

        private void SubmitButtonClicked(jQueryEvent e)
        {
            // return if anything is checking
            if (MySqlSettingChecking || SevenZipSettingChecking || PdfinfoSettingChecking || MudrawSettingChecking)
            {
                return;
            }

            if (!AllRequiredComponentLoaded)
            {
                ErrorModal.ShowError(Strings.Get("MissingRequiredComponent"));
                return;
            }

            if (!CanEnableZip && !CanEnableRar && !CanEnablePdf)
            {
                ErrorModal.ShowError(Strings.Get("NeedFileSupport"));
                return;
            }

            string username = jQuery.Select("#install-admin-username").GetValue();
            string password = jQuery.Select("#install-admin-password").GetValue();
            string password2 = jQuery.Select("#install-admin-password2").GetValue();

            RegularExpression regex = new RegularExpression("[^a-zA-Z0-9]");

            if (username == "" || regex.Test(username) || password.Length < 8 || password != password2)
            {
                ErrorModal.ShowError(Strings.Get("AdminUserSettingFailed"));
                return;
            }

            InstallRequest request = new InstallRequest();
            request.mysqlServer = jQuery.Select("#install-mysql-server").GetValue();
            request.mysqlPort = int.Parse(jQuery.Select("#install-mysql-port").GetValue(), 10);
            request.mysqlUser = jQuery.Select("#install-mysql-username").GetValue();
            request.mysqlPassword = jQuery.Select("#install-mysql-password").GetValue();
            request.mysqlDatabase = jQuery.Select("#install-mysql-database").GetValue();

            request.sevenZipPath = jQuery.Select("#install-sevenzip-dll").GetValue();
            request.pdfinfoPath = jQuery.Select("#install-pdfinfo-exe").GetValue();
            request.mudrawPath = jQuery.Select("#install-mudraw-exe").GetValue();

            request.zip = jQuery.Select("#install-zip-checkbox").GetAttribute("checked") == "checked";
            request.rar = jQuery.Select("#install-rar-checkbox").GetAttribute("checked") == "checked";
            request.pdf = jQuery.Select("#install-pdf-checkbox").GetAttribute("checked") == "checked";

            request.admin = username;
            request.password = password;
            request.password2 = password2;

            new InstallingModal().SendInstallRequest(request);
        }

        [AlternateSignature]
        private extern void LanguageChange(jQueryEvent e);
        private void LanguageChange()
        {
            string newLanguage = jQuery.Select("#install-language").GetValue();
            if (newLanguage != Settings.UserLanguage)
            {
                Settings.UserLanguage = newLanguage;
                Window.Location.Href = "install.html";
            }
        }
    }
}
