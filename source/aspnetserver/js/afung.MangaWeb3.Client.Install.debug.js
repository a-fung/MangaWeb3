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
        afung.MangaWeb3.Client.Template.templateIds['install'] = [ 'installing-modal', 'install-mudraw-error', 'install-pdfdraw-error', 'install-pdfinfo-error', 'install-pdfinfoexe-error', 'install-rar-error', 'install-zip-error', 'install-sevenzip-error', 'install-gd-error', 'install-mysql-connect-error', 'install-mysql-error', 'install-module' ];
        afung.MangaWeb3.Client.Request.endpoint = 'InstallAjax';
        afung.MangaWeb3.Client.Install.InstallApp.callBaseMethod(this, 'startStage2');
    },
    
    loadFirstModule: function afung_MangaWeb3_Client_Install_InstallApp$loadFirstModule() {
        new afung.MangaWeb3.Client.Install.Module.InstallModule();
    }
}


Type.registerNamespace('afung.MangaWeb3.Client.Install.Modal');

////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Install.Modal.InstallingModal

afung.MangaWeb3.Client.Install.Modal.InstallingModal = function afung_MangaWeb3_Client_Install_Modal_InstallingModal(request) {
    /// <param name="request" type="afung.MangaWeb3.Common.InstallRequest">
    /// </param>
    /// <field name="_request$1" type="afung.MangaWeb3.Common.InstallRequest">
    /// </field>
    afung.MangaWeb3.Client.Install.Modal.InstallingModal.initializeBase(this, [ 'install', 'installing-modal' ]);
    this._request$1 = request;
}
afung.MangaWeb3.Client.Install.Modal.InstallingModal.prototype = {
    _request$1: null,
    
    initialize: function afung_MangaWeb3_Client_Install_Modal_InstallingModal$initialize() {
        this.showStatic();
        afung.MangaWeb3.Client.Request.send(this._request$1, ss.Delegate.create(this, this._installRequestSuccess$1));
    },
    
    _installRequestSuccess$1: function afung_MangaWeb3_Client_Install_Modal_InstallingModal$_installRequestSuccess$1() {
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
    /// <field name="_sevenZipSettingChecking$1" type="Boolean">
    /// </field>
    /// <field name="_sevenZipSettingOkay$1" type="Boolean">
    /// </field>
    /// <field name="_pdfinfoSettingChecking$1" type="Boolean">
    /// </field>
    /// <field name="_pdfinfoSettingOkay$1" type="Boolean">
    /// </field>
    /// <field name="_mudrawSettingChecking$1" type="Boolean">
    /// </field>
    /// <field name="_mudrawSettingOkay$1" type="Boolean">
    /// </field>
    /// <field name="_checkingComponent$1" type="Number" integer="true">
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
            $('#install-sevenzip-dll').change(ss.Delegate.create(this, this._otherComponentInputChanged$1));
            $('#install-pdfinfoexe').change(ss.Delegate.create(this, this._otherComponentInputChanged$1));
            $('#install-mudraw-exe').change(ss.Delegate.create(this, this._otherComponentInputChanged$1));
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
        $('#install-admin-username').change(ss.Delegate.create(this, this._adminUserChanged$1));
        $('#install-admin-password').change(ss.Delegate.create(this, this._adminPasswordChanged$1));
        $('#install-admin-password2').change(ss.Delegate.create(this, this._adminConfirmPasswordChanged$1));
        $('#install-submit-btn').click(ss.Delegate.create(this, this._submitButtonClicked$1));
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
            this._mySqlCheckFailed$1();
        }
    },
    
    _mySqlCheckFailed$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_mySqlCheckFailed$1() {
        this.set__mySqlSettingOkay$1(this.set__mySqlSettingChecking$1(false));
        afung.MangaWeb3.Client.Template.get('install', 'install-mysql-connect-error').appendTo($('#mysql-error-area'));
    },
    
    _sevenZipSettingChecking$1: false,
    
    get__sevenZipSettingChecking$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__sevenZipSettingChecking$1() {
        /// <value type="Boolean"></value>
        return this._sevenZipSettingChecking$1;
    },
    set__sevenZipSettingChecking$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__sevenZipSettingChecking$1(value) {
        /// <value type="Boolean"></value>
        this._sevenZipSettingChecking$1 = value;
        if (value) {
            $('#install-sevenzip-loading').show();
            $('#install-sevenzip-dll').attr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled, afung.MangaWeb3.Client.HtmlConstants.attributeDisabled);
        }
        else {
            $('#install-sevenzip-loading').hide();
            $('#install-sevenzip-dll').removeAttr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled);
        }
        return value;
    },
    
    _sevenZipSettingOkay$1: false,
    
    get__sevenZipSettingOkay$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__sevenZipSettingOkay$1() {
        /// <value type="Boolean"></value>
        return this._sevenZipSettingOkay$1;
    },
    set__sevenZipSettingOkay$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__sevenZipSettingOkay$1(value) {
        /// <value type="Boolean"></value>
        this.set__canEnableZip$1(this.set__canEnableRar$1(this._sevenZipSettingOkay$1 = value));
        if (value) {
            $('#install-sevenzip-check').show();
        }
        else {
            $('#install-sevenzip-check').hide();
        }
        return value;
    },
    
    _pdfinfoSettingChecking$1: false,
    
    get__pdfinfoSettingChecking$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__pdfinfoSettingChecking$1() {
        /// <value type="Boolean"></value>
        return this._pdfinfoSettingChecking$1;
    },
    set__pdfinfoSettingChecking$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__pdfinfoSettingChecking$1(value) {
        /// <value type="Boolean"></value>
        this._pdfinfoSettingChecking$1 = value;
        if (value) {
            $('#install-pdfinfoexe-loading').show();
            $('#install-pdfinfoexe').attr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled, afung.MangaWeb3.Client.HtmlConstants.attributeDisabled);
        }
        else {
            $('#install-pdfinfoexe-loading').hide();
            $('#install-pdfinfoexe').removeAttr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled);
        }
        return value;
    },
    
    _pdfinfoSettingOkay$1: false,
    
    get__pdfinfoSettingOkay$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__pdfinfoSettingOkay$1() {
        /// <value type="Boolean"></value>
        return this._pdfinfoSettingOkay$1;
    },
    set__pdfinfoSettingOkay$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__pdfinfoSettingOkay$1(value) {
        /// <value type="Boolean"></value>
        this._pdfinfoSettingOkay$1 = value;
        this.set__canEnablePdf$1(value && this.get__mudrawSettingOkay$1());
        if (value) {
            $('#install-pdfinfoexe-check').show();
        }
        else {
            $('#install-pdfinfoexe-check').hide();
        }
        return value;
    },
    
    _mudrawSettingChecking$1: false,
    
    get__mudrawSettingChecking$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__mudrawSettingChecking$1() {
        /// <value type="Boolean"></value>
        return this._mudrawSettingChecking$1;
    },
    set__mudrawSettingChecking$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__mudrawSettingChecking$1(value) {
        /// <value type="Boolean"></value>
        this._mudrawSettingChecking$1 = value;
        if (value) {
            $('#install-mudraw-loading').show();
            $('#install-mudraw-exe').attr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled, afung.MangaWeb3.Client.HtmlConstants.attributeDisabled);
        }
        else {
            $('#install-mudraw-loading').hide();
            $('#install-mudraw-exe').removeAttr(afung.MangaWeb3.Client.HtmlConstants.attributeDisabled);
        }
        return value;
    },
    
    _mudrawSettingOkay$1: false,
    
    get__mudrawSettingOkay$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$get__mudrawSettingOkay$1() {
        /// <value type="Boolean"></value>
        return this._mudrawSettingOkay$1;
    },
    set__mudrawSettingOkay$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$set__mudrawSettingOkay$1(value) {
        /// <value type="Boolean"></value>
        this._mudrawSettingOkay$1 = value;
        this.set__canEnablePdf$1(value && this.get__pdfinfoSettingOkay$1());
        if (value) {
            $('#install-mudraw-check').show();
        }
        else {
            $('#install-mudraw-check').hide();
        }
        return value;
    },
    
    _checkingComponent$1: 0,
    
    _otherComponentInputChanged$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_otherComponentInputChanged$1(e) {
        /// <param name="e" type="jQueryEvent">
        /// </param>
        var eventSource = $(e.target);
        var inputId = eventSource.attr(afung.MangaWeb3.Client.HtmlConstants.attributeId);
        var checking = false;
        switch (inputId) {
            case 'install-sevenzip-dll':
                checking = this.get__sevenZipSettingChecking$1();
                break;
            case 'install-pdfinfoexe':
                checking = this.get__pdfinfoSettingChecking$1();
                break;
            case 'install-mudraw-exe':
                checking = this.get__mudrawSettingChecking$1();
                break;
            default:
                return;
        }
        if (checking) {
            return;
        }
        var path = eventSource.val();
        checking = !String.isNullOrEmpty(path);
        var request = new afung.MangaWeb3.Common.CheckOtherComponentRequest();
        request.path = path;
        switch (inputId) {
            case 'install-sevenzip-dll':
                this.set__sevenZipSettingChecking$1(checking);
                this.set__sevenZipSettingOkay$1(false);
                request.component = 0;
                $('#install-sevenzip-error').remove();
                break;
            case 'install-pdfinfoexe':
                this.set__pdfinfoSettingChecking$1(checking);
                this.set__pdfinfoSettingOkay$1(false);
                request.component = 1;
                $('#install-pdfinfoexe-error').remove();
                break;
            case 'install-mudraw-exe':
                this.set__mudrawSettingChecking$1(checking);
                this.set__mudrawSettingOkay$1(false);
                request.component = 2;
                $('#install-mudraw-error').remove();
                break;
            default:
                return;
        }
        if (!checking) {
            return;
        }
        this._checkingComponent$1 = request.component;
        afung.MangaWeb3.Client.Request.send(request, ss.Delegate.create(this, this._otherComponentCheckSuccess$1), ss.Delegate.create(this, this._otherComponentCheckFailed$1));
    },
    
    _otherComponentCheckSuccess$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_otherComponentCheckSuccess$1(response) {
        /// <param name="response" type="afung.MangaWeb3.Common.CheckOtherComponentResponse">
        /// </param>
        if (response.pass) {
            switch (this._checkingComponent$1) {
                case 0:
                    this.set__sevenZipSettingChecking$1(false);
                    this.set__sevenZipSettingOkay$1(true);
                    break;
                case 1:
                    this.set__pdfinfoSettingChecking$1(false);
                    this.set__pdfinfoSettingOkay$1(true);
                    break;
                case 2:
                    this.set__mudrawSettingChecking$1(false);
                    this.set__mudrawSettingOkay$1(true);
                    break;
                default:
                    return;
            }
        }
        else {
            this._otherComponentCheckFailed$1();
        }
    },
    
    _otherComponentCheckFailed$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_otherComponentCheckFailed$1() {
        switch (this._checkingComponent$1) {
            case 0:
                this.set__sevenZipSettingOkay$1(this.set__sevenZipSettingChecking$1(false));
                afung.MangaWeb3.Client.Template.get('install', 'install-sevenzip-error').appendTo($('#sevenzip-error-area'));
                break;
            case 1:
                this.set__pdfinfoSettingOkay$1(this.set__pdfinfoSettingChecking$1(false));
                afung.MangaWeb3.Client.Template.get('install', 'install-pdfinfoexe-error').appendTo($('#pdfinfo-error-area'));
                break;
            case 2:
                this.set__mudrawSettingOkay$1(this.set__mudrawSettingChecking$1(false));
                afung.MangaWeb3.Client.Template.get('install', 'install-mudraw-error').appendTo($('#mudraw-error-area'));
                break;
            default:
                return;
        }
    },
    
    _adminUserChanged$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_adminUserChanged$1(e) {
        /// <param name="e" type="jQueryEvent">
        /// </param>
        var regex = new RegExp('[^a-zA-Z0-9]');
        var username = $('#install-admin-username').val();
        if (!username || regex.test(username)) {
            $('#install-admin-username-check').hide();
        }
        else {
            $('#install-admin-username-check').show();
        }
    },
    
    _adminPasswordChanged$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_adminPasswordChanged$1(e) {
        /// <param name="e" type="jQueryEvent">
        /// </param>
        if ($('#install-admin-password').val().length >= 8) {
            $('#install-admin-password-check').show();
        }
        else {
            $('#install-admin-password-check').hide();
        }
    },
    
    _adminConfirmPasswordChanged$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_adminConfirmPasswordChanged$1(e) {
        /// <param name="e" type="jQueryEvent">
        /// </param>
        if ($('#install-admin-password2').val().length >= 8 && $('#install-admin-password').val() === $('#install-admin-password2').val()) {
            $('#install-admin-password2-check').show();
        }
        else {
            $('#install-admin-password2-check').hide();
        }
    },
    
    _submitButtonClicked$1: function afung_MangaWeb3_Client_Install_Module_InstallModule$_submitButtonClicked$1(e) {
        /// <param name="e" type="jQueryEvent">
        /// </param>
        if (this.get__mySqlSettingChecking$1() || this.get__sevenZipSettingChecking$1() || this.get__pdfinfoSettingChecking$1() || this.get__mudrawSettingChecking$1()) {
            return;
        }
        if (!this._allRequiredComponentLoaded$1) {
            afung.MangaWeb3.Client.Modal.ErrorModal.showError(afung.MangaWeb3.Client.Strings.get('MissingRequiredComponent'));
            return;
        }
        if (!this.get__canEnableZip$1() && !this.get__canEnableRar$1() && !this.get__canEnablePdf$1()) {
            afung.MangaWeb3.Client.Modal.ErrorModal.showError(afung.MangaWeb3.Client.Strings.get('NeedFileSupport'));
            return;
        }
        var username = $('#install-admin-username').val();
        var password = $('#install-admin-password').val();
        var password2 = $('#install-admin-password2').val();
        var regex = new RegExp('[^a-zA-Z0-9]');
        if (!username || regex.test(username) || password.length < 8 || password !== password2) {
            afung.MangaWeb3.Client.Modal.ErrorModal.showError(afung.MangaWeb3.Client.Strings.get('AdminUserSettingFailed'));
            return;
        }
        var request = new afung.MangaWeb3.Common.InstallRequest();
        request.mysqlServer = $('#install-mysql-server').val();
        request.mysqlPort = parseInt($('#install-mysql-port').val(), 10);
        request.mysqlUser = $('#install-mysql-username').val();
        request.mysqlPassword = $('#install-mysql-password').val();
        request.mysqlDatabase = $('#install-mysql-database').val();
        request.sevenZipPath = $('#install-sevenzip-dll').val();
        request.pdfinfoPath = $('#install-pdfinfoexe').val();
        request.mudrawPath = $('#install-mudraw-exe').val();
        request.zip = $('#install-zip-checkbox').attr(afung.MangaWeb3.Client.HtmlConstants.attributeChecked) === afung.MangaWeb3.Client.HtmlConstants.attributeChecked;
        request.rar = $('#install-rar-checkbox').attr(afung.MangaWeb3.Client.HtmlConstants.attributeChecked) === afung.MangaWeb3.Client.HtmlConstants.attributeChecked;
        request.pdf = $('#install-pdf-checkbox').attr(afung.MangaWeb3.Client.HtmlConstants.attributeChecked) === afung.MangaWeb3.Client.HtmlConstants.attributeChecked;
        request.admin = username;
        request.password = password;
        request.password2 = password2;
        new afung.MangaWeb3.Client.Install.Modal.InstallingModal(request);
    }
}


afung.MangaWeb3.Client.Install.InstallApp.registerClass('afung.MangaWeb3.Client.Install.InstallApp', afung.MangaWeb3.Client.Application);
afung.MangaWeb3.Client.Install.Modal.InstallingModal.registerClass('afung.MangaWeb3.Client.Install.Modal.InstallingModal', afung.MangaWeb3.Client.Modal.ModalBase);
afung.MangaWeb3.Client.Install.Module.InstallModule.registerClass('afung.MangaWeb3.Client.Install.Module.InstallModule', afung.MangaWeb3.Client.Module.ModuleBase);
})();

//! This script was generated using Script# v0.7.4.0
