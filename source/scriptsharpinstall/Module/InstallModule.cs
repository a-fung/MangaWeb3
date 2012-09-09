// InstallModule.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
                    jQuery.Select("#install-zip-checkbox", attachedObject).RemoveAttr(HtmlConstants.AttributeDisabled);
                }
                else
                {
                    jQuery.Select("#install-zip-checkbox", attachedObject)
                        .Attribute(HtmlConstants.AttributeDisabled, HtmlConstants.AttributeDisabled)
                        .RemoveAttr(HtmlConstants.AttributeChecked);
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
                    jQuery.Select("#install-rar-checkbox", attachedObject).RemoveAttr(HtmlConstants.AttributeDisabled);
                }
                else
                {
                    jQuery.Select("#install-rar-checkbox", attachedObject)
                        .Attribute(HtmlConstants.AttributeDisabled, HtmlConstants.AttributeDisabled)
                        .RemoveAttr(HtmlConstants.AttributeChecked);
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
                    jQuery.Select("#install-pdf-checkbox", attachedObject).RemoveAttr(HtmlConstants.AttributeDisabled);
                }
                else
                {
                    jQuery.Select("#install-pdf-checkbox", attachedObject)
                        .Attribute(HtmlConstants.AttributeDisabled, HtmlConstants.AttributeDisabled)
                        .RemoveAttr(HtmlConstants.AttributeChecked);
                }
                _canEnablePdf = value;
            }
        }

        private bool AllRequiredComponentLoaded;

        public InstallModule()
            : base("install", "install-module")
        {
        }

        protected override void Initialize()
        {
            // hide install form and send out a request to check if MangaWeb is installed
            jQuery.Select("#install-form").Hide();
            Request.Send(new PreInstallCheckRequest(), OnPreInstallCheckRequestFinished);
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

            jQuery.Select("#install-mysql-check-setting").Click(MySqlCheckSettingClicked);

            jQuery.Select("#install-mysql-server").Change(MySqlCheckSettingChanged);
            jQuery.Select("#install-mysql-port").Change(MySqlCheckSettingChanged);
            jQuery.Select("#install-mysql-username").Change(MySqlCheckSettingChanged);
            jQuery.Select("#install-mysql-password").Change(MySqlCheckSettingChanged);
            jQuery.Select("#install-mysql-database").Change(MySqlCheckSettingChanged);
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
                MySqlCheckFailed(new Exception(""));
            }
        }

        private void MySqlCheckFailed(Exception error)
        {
            MySqlSettingOkay = MySqlSettingChecking = false;
            Template.Get("install", "install-mysql-connect-error").AppendTo(jQuery.Select("#mysql-error-area"));

        }
    }
}
