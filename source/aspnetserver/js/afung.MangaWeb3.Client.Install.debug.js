//! afung.MangaWeb3.Client.Install.debug.js
//

(function() {

Type.registerNamespace('afung.MangaWeb3.Client.Install');

////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Install.InstallApp

afung.MangaWeb3.Client.Install.InstallApp = function afung_MangaWeb3_Client_Install_InstallApp() {
    /// <summary>
    /// Class InstallApp
    /// </summary>
    afung.MangaWeb3.Client.Install.InstallApp.initializeBase(this);
}
afung.MangaWeb3.Client.Install.InstallApp.prototype = {
    
    startStage2: function afung_MangaWeb3_Client_Install_InstallApp$startStage2() {
        afung.MangaWeb3.Client.Template.templates[afung.MangaWeb3.Client.Template.templates.length] = 'install';
        afung.MangaWeb3.Client.Template.templateIds['install'] = [ 'install-mudraw-error', 'install-pdfdraw-error', 'install-pdfinfo-error', 'install-pdfinfoexe-error', 'install-rar-error', 'install-zip-error', 'install-sevenzip-error', 'install-gd-error', 'install-mysql-connect-error', 'install-mysql-error', 'install-module' ];
        afung.MangaWeb3.Client.Request.endpoint = 'InstallAjax';
        afung.MangaWeb3.Client.Install.InstallApp.callBaseMethod(this, 'startStage2');
    },
    
    loadFirstModule: function afung_MangaWeb3_Client_Install_InstallApp$loadFirstModule() {
        new afung.MangaWeb3.Client.Install.Module.InstallModule();
    }
}


Type.registerNamespace('afung.MangaWeb3.Client.Install.Module');

////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Install.Module.InstallModule

afung.MangaWeb3.Client.Install.Module.InstallModule = function afung_MangaWeb3_Client_Install_Module_InstallModule() {
    /// <field name="_canEnableZip$1" type="Boolean">
    /// </field>
    /// <field name="_canEnableRar$1" type="Boolean">
    /// </field>
    /// <field name="_canEnablePdf$1" type="Boolean">
    /// </field>
    /// <field name="_allRequiredComponentLoaded$1" type="Boolean">
    /// </field>
    /// <field name="_mySqlSettingChecking$1" type="Boolean">
    /// </field>
    /// <field name="_mySqlSettingOkay$1" type="Boolean">
    /// </field>
    afung.MangaWeb3.Client.Install.Module.InstallModule.initializeBase(this, [ 'install', 'install-module' ]);
}
afung.MangaWeb3.Client.Install.Module.InstallModule.prototype = {
    _canEnableZip$1: false,
    
    get__canEnableZip$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__canEnableZip$1() {
        /// <value type="Boolean"></value>
        return this._canEnableZip$1;
    },
    set__canEnableZip$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__canEnableZip$1(value) {
        /// <value type="Boolean"></value>
        if (value) {
            $('#install-zip-checkbox', this.attachedObject).removeAttr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled);
        }
        else {
            $('#install-zip-checkbox', this.attachedObject).attr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled, afung.MangaWeb3.Client.HtmlConstants.attributeDisabled).removeAttr(afung.MangaWeb3.Client.HtmlConstants.attributeChecked);
        }
        this._canEnableZip$1 = value;
        return value;
    },
    
    _canEnableRar$1: false,
    
    get__canEnableRar$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__canEnableRar$1() {
        /// <value type="Boolean"></value>
        return this._canEnableRar$1;
    },
    set__canEnableRar$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__canEnableRar$1(value) {
        /// <value type="Boolean"></value>
        if (value) {
            $('#install-rar-checkbox', this.attachedObject).removeAttr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled);
        }
        else {
            $('#install-rar-checkbox', this.attachedObject).attr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled, afung.MangaWeb3.Client.HtmlConstants.attributeDisabled).removeAttr(afung.MangaWeb3.Client.HtmlConstants.attributeChecked);
        }
        this._canEnableRar$1 = value;
        return value;
    },
    
    _canEnablePdf$1: false,
    
    get__canEnablePdf$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__canEnablePdf$1() {
        /// <value type="Boolean"></value>
        return this._canEnablePdf$1;
    },
    set__canEnablePdf$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__canEnablePdf$1(value) {
        /// <value type="Boolean"></value>
        if (value) {
            $('#install-pdf-checkbox', this.attachedObject).removeAttr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled);
        }
        else {
            $('#install-pdf-checkbox', this.attachedObject).attr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled, afung.MangaWeb3.Client.HtmlConstants.attributeDisabled).removeAttr(afung.MangaWeb3.Client.HtmlConstants.attributeChecked);
        }
        this._canEnablePdf$1 = value;
        return value;
    },
    
    _allRequiredComponentLoaded$1: false,
    
    initialize: function afung_MangaWeb3_Client_Install_Module_InstallModule$initialize() {
        $('#install-form').hide();
        afung.MangaWeb3.Client.Request.send(new afung.MangaWeb3.Common.PreInstallCheckRequest(), ss.Delegate.create(this, this._onPreInstallCheckRequestFinished$1));
    },
    
    _onPreInstallCheckRequestFinished$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_onPreInstallCheckRequestFinished$1(response) {
        /// <param name="response" type="afung.MangaWeb3.Common.PreInstallCheckResponse">
        /// </param>
        if (response.installed) {
            return;
        }
        $('#install-preinstall-check').hide();
        $('#install-form').show();
        $('#install-mysql-check').hide();
        $('#install-mysql-loading').hide();
        $('#install-sevenzip-check').hide();
        $('#install-sevenzip-loading').hide();
        $('#install-pdfinfoexe-check').hide();
        $('#install-pdfinfoexe-loading').hide();
        $('#install-mudraw-check').hide();
        $('#install-mudraw-loading').hide();
        $('#install-admin-username-check').hide();
        $('#install-admin-password-check').hide();
        $('#install-admin-password2-check').hide();
        if (afung.MangaWeb3.Client.Environment.serverType === afung.MangaWeb3.Client.ServerType.aspNet) {
            this.set__canEnableZip$1(this.set__canEnableRar$1(this.set__canEnablePdf$1(false)));
            this._allRequiredComponentLoaded$1 = true;
        }
        else {
            $('#install-sevenzip').hide();
            $('#install-pdfinfoexe').hide();
            $('#install-mudraw').hide();
            this.set__canEnableZip$1(response.zip);
            this.set__canEnableRar$1(response.rar);
            this.set__canEnablePdf$1(response.pdfinfo && response.pdfdraw);
            this._allRequiredComponentLoaded$1 = response.mySql && response.gd;
            if (!response.mySql) {
                afung.MangaWeb3.Client.Template.get('install', 'install-mysql-error').appendTo($('#mysql-error-area'));
            }
            if (!response.gd) {
                afung.MangaWeb3.Client.Template.get('install', 'install-gd-error').appendTo($('#gd-error-area'));
            }
            if (!response.zip) {
                afung.MangaWeb3.Client.Template.get('install', 'install-zip-error').appendTo($('#zip-error-area'));
            }
            if (!response.rar) {
                afung.MangaWeb3.Client.Template.get('install', 'install-rar-error').appendTo($('#rar-error-area'));
            }
            if (!response.pdfinfo) {
                afung.MangaWeb3.Client.Template.get('install', 'install-pdfinfo-error').appendTo($('#pdfinfo-error-area'));
            }
            if (!response.pdfdraw) {
                afung.MangaWeb3.Client.Template.get('install', 'install-pdfdraw-error').appendTo($('#mudraw-error-area'));
            }
        }
        $('#install-mysql-check-setting').click(ss.Delegate.create(this, this._mySqlCheckSettingClicked$1));
        $('#install-mysql-server').change(ss.Delegate.create(this, this._mySqlCheckSettingChanged$1));
        $('#install-mysql-port').change(ss.Delegate.create(this, this._mySqlCheckSettingChanged$1));
        $('#install-mysql-username').change(ss.Delegate.create(this, this._mySqlCheckSettingChanged$1));
        $('#install-mysql-password').change(ss.Delegate.create(this, this._mySqlCheckSettingChanged$1));
        $('#install-mysql-database').change(ss.Delegate.create(this, this._mySqlCheckSettingChanged$1));
    },
    
    _mySqlSettingChecking$1: false,
    
    get__mySqlSettingChecking$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__mySqlSettingChecking$1() {
        /// <value type="Boolean"></value>
        return this._mySqlSettingChecking$1;
    },
    set__mySqlSettingChecking$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__mySqlSettingChecking$1(value) {
        /// <value type="Boolean"></value>
        this._mySqlSettingChecking$1 = value;
        if (value) {
            $('#install-mysql-loading').show();
        }
        else {
            $('#install-mysql-loading').hide();
        }
        return value;
    },
    
    _mySqlSettingOkay$1: false,
    
    get__mySqlSettingOkay$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__mySqlSettingOkay$1() {
        /// <value type="Boolean"></value>
        return this._mySqlSettingOkay$1;
    },
    set__mySqlSettingOkay$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__mySqlSettingOkay$1(value) {
        /// <value type="Boolean"></value>
        this._mySqlSettingOkay$1 = value;
        if (value) {
            $('#install-mysql-check').show();
        }
        else {
            $('#install-mysql-check').hide();
        }
        return value;
    },
    
    _mySqlCheckSettingChanged$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_mySqlCheckSettingChanged$1(e) {
        /// <param name="e" type="jQueryEvent">
        /// </param>
        this.set__mySqlSettingOkay$1(false);
    },
    
    _mySqlCheckSettingClicked$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_mySqlCheckSettingClicked$1(e) {
        /// <param name="e" type="jQueryEvent">
        /// </param>
        if (this.get__mySqlSettingChecking$1()) {
            return;
        }
        var server = $('#install-mysql-server').val();
        var portString = $('#install-mysql-port').val();
        var username = $('#install-mysql-username').val();
        var password = $('#install-mysql-password').val();
        var database = $('#install-mysql-database').val();
        if (String.isNullOrEmpty(server) || String.isNullOrEmpty(portString) || String.isNullOrEmpty(username) || String.isNullOrEmpty(database)) {
            return;
        }
        var port = parseInt(portString, 10);
        var request = new afung.MangaWeb3.Common.CheckMySqlSettingRequest();
        request.server = server;
        request.port = port;
        request.username = username;
        request.password = password;
        request.database = database;
        $('#install-mysql-connect-error').remove();
        this.set__mySqlSettingChecking$1(true);
        this.set__mySqlSettingOkay$1(false);
        afung.MangaWeb3.Client.Request.send(request, ss.Delegate.create(this, this._mysqlCheckSuccess$1), ss.Delegate.create(this, this._mySqlCheckFailed$1));
    },
    
    _mysqlCheckSuccess$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_mysqlCheckSuccess$1(response) {
        /// <param name="response" type="afung.MangaWeb3.Common.CheckMySqlSettingResponse">
        /// </param>
        this.set__mySqlSettingChecking$1(false);
        this.set__mySqlSettingOkay$1(response.pass);
        if (!response.pass) {
            this._mySqlCheckFailed$1(new Error(''));
        }
    },
    
    _mySqlCheckFailed$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_mySqlCheckFailed$1(error) {
        /// <param name="error" type="Error">
        /// </param>
        this.set__mySqlSettingOkay$1(this.set__mySqlSettingChecking$1(false));
        afung.MangaWeb3.Client.Template.get('install', 'install-mysql-connect-error').appendTo($('#mysql-error-area'));
    }
}


afung.MangaWeb3.Client.Install.InstallApp.registerClass('afung.MangaWeb3.Client.Install.InstallApp', afung.MangaWeb3.Client.Application);
afung.MangaWeb3.Client.Install.Module.InstallModule.registerClass('afung.MangaWeb3.Client.Install.Module.InstallModule', afung.MangaWeb3.Client.Module.ModuleBase);
})();

//! This script was generated using Script# v0.7.4.0
