//! afung.MangaWeb3.Client.debug.js
//

(function() {

Type.registerNamespace('afung.MangaWeb3.Client.Modal');

////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Modal.ErrorModal

afung.MangaWeb3.Client.Modal.ErrorModal = function afung_MangaWeb3_Client_Modal_ErrorModal() {
    /// <field name="_instance$1" type="afung.MangaWeb3.Client.Modal.ErrorModal" static="true">
    /// </field>
    afung.MangaWeb3.Client.Modal.ErrorModal.initializeBase(this, [ 'client', 'error-modal' ]);
}
afung.MangaWeb3.Client.Modal.ErrorModal.showError = function afung_MangaWeb3_Client_Modal_ErrorModal$showError(errorMsg) {
    /// <param name="errorMsg" type="String">
    /// </param>
    if (afung.MangaWeb3.Client.Modal.ErrorModal._instance$1 == null) {
        afung.MangaWeb3.Client.Modal.ErrorModal._instance$1 = new afung.MangaWeb3.Client.Modal.ErrorModal();
    }
    afung.MangaWeb3.Client.Modal.ErrorModal._instance$1.internalShowError(errorMsg);
}
afung.MangaWeb3.Client.Modal.ErrorModal.prototype = {
    
    initialize: function afung_MangaWeb3_Client_Modal_ErrorModal$initialize() {
    },
    
    internalShowError: function afung_MangaWeb3_Client_Modal_ErrorModal$internalShowError(errorMsg) {
        /// <param name="errorMsg" type="String">
        /// </param>
        $('#error-modal-msg', this.attachedObject).text(errorMsg);
        this.show();
    }
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Modal.ModalBase

afung.MangaWeb3.Client.Modal.ModalBase = function afung_MangaWeb3_Client_Modal_ModalBase(template, templateId) {
    /// <param name="template" type="String">
    /// </param>
    /// <param name="templateId" type="String">
    /// </param>
    /// <field name="attachedObject" type="jQueryObject">
    /// </field>
    this.attachedObject = afung.MangaWeb3.Client.Template.get(template, templateId).appendTo($('body'));
    this.initialize();
}
afung.MangaWeb3.Client.Modal.ModalBase.prototype = {
    attachedObject: null,
    
    show: function afung_MangaWeb3_Client_Modal_ModalBase$show() {
        (this.attachedObject).modal();
    }
}


Type.registerNamespace('afung.MangaWeb3.Client');

////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.ServerType

afung.MangaWeb3.Client.ServerType = function() { 
    /// <summary>
    /// Server Type enum
    /// </summary>
    /// <field name="aspNet" type="Number" integer="true" static="true">
    /// The server is running ASP.NET
    /// </field>
    /// <field name="php" type="Number" integer="true" static="true">
    /// The server is running PHP
    /// </field>
};
afung.MangaWeb3.Client.ServerType.prototype = {
    aspNet: 0, 
    php: 1
}
afung.MangaWeb3.Client.ServerType.registerEnum('afung.MangaWeb3.Client.ServerType', false);


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Request

afung.MangaWeb3.Client.Request = function afung_MangaWeb3_Client_Request(data, successCallback, failureCallback) {
    /// <param name="data" type="afung.MangaWeb3.Common.JsonRequest">
    /// </param>
    /// <param name="successCallback" type="System.Action`1">
    /// </param>
    /// <param name="failureCallback" type="System.Action`1">
    /// </param>
    /// <field name="endpoint" type="String" static="true">
    /// </field>
    /// <field name="_data" type="afung.MangaWeb3.Common.JsonRequest">
    /// </field>
    /// <field name="_successCallback" type="System.Action`1">
    /// </field>
    /// <field name="_failureCallback" type="System.Action`1">
    /// </field>
    this._data = data;
    this._successCallback = successCallback;
    this._failureCallback = failureCallback;
}
afung.MangaWeb3.Client.Request.send = function afung_MangaWeb3_Client_Request$send(data, successCallback, failureCallback) {
    /// <param name="data" type="afung.MangaWeb3.Common.JsonRequest">
    /// </param>
    /// <param name="successCallback" type="System.Action`1">
    /// </param>
    /// <param name="failureCallback" type="System.Action`1">
    /// </param>
    var request = new afung.MangaWeb3.Client.Request(data, successCallback, failureCallback);
    request._sendRequest();
}
afung.MangaWeb3.Client.Request.prototype = {
    _data: null,
    _successCallback: null,
    _failureCallback: null,
    
    _sendRequest: function afung_MangaWeb3_Client_Request$_sendRequest() {
        var options = {};
        options.data = { j: JSON.stringify(this._data) };
        options.dataType = 'json';
        options.type = 'POST';
        options.cache = false;
        options.error = ss.Delegate.create(this, this._ajaxError);
        options.success = ss.Delegate.create(this, this._ajaxSuccess);
        $.ajax(afung.MangaWeb3.Client.Request.endpoint + ((!afung.MangaWeb3.Client.Environment.serverType) ? '.aspx' : '.php'), options);
    },
    
    _ajaxError: function afung_MangaWeb3_Client_Request$_ajaxError(request, textStatus, error) {
        /// <param name="request" type="jQueryXmlHttpRequest">
        /// </param>
        /// <param name="textStatus" type="String">
        /// </param>
        /// <param name="error" type="Error">
        /// </param>
        this._failure(error);
    },
    
    _ajaxSuccess: function afung_MangaWeb3_Client_Request$_ajaxSuccess(responseData, textStatus, request) {
        /// <param name="responseData" type="Object">
        /// </param>
        /// <param name="textStatus" type="String">
        /// </param>
        /// <param name="request" type="jQueryXmlHttpRequest">
        /// </param>
        if (ss.isNullOrUndefined(responseData)) {
            return;
        }
        var error = responseData;
        if (ss.isValue(error.errorCode) || ss.isValue(error.errorMsg)) {
            this._failure(new Error(afung.MangaWeb3.Client.Strings.get(error.errorMsg)));
            return;
        }
        this._success(responseData);
    },
    
    _failure: function afung_MangaWeb3_Client_Request$_failure(error) {
        /// <param name="error" type="Error">
        /// </param>
        if (ss.isValue(this._failureCallback)) {
            this._failureCallback(error);
            return;
        }
        afung.MangaWeb3.Client.Modal.ErrorModal.showError(String.format(afung.MangaWeb3.Client.Strings.get('RequestFailed'), error));
    },
    
    _success: function afung_MangaWeb3_Client_Request$_success(responseData) {
        /// <param name="responseData" type="Object">
        /// </param>
        if (ss.isValue(this._successCallback)) {
            this._successCallback(responseData);
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Application

afung.MangaWeb3.Client.Application = function afung_MangaWeb3_Client_Application() {
    /// <summary>
    /// Class Application
    /// </summary>
    /// <field name="_fatalErrorMessageHtml" type="String" static="true">
    /// The HTML for showing fatal error message
    /// </field>
    /// <field name="_fatalErrorMessageClass" type="String" static="true">
    /// The class for showing fatal error message inside the above HTML
    /// </field>
    var defaultLanguageLoadFailed = function(error) {
        afung.MangaWeb3.Client.Application.showFatalError(String.format('Unabled to load default language file ({0}). Please check that you have the correct file on your server', error));
    };
    var defaultLanguageLoadFinished = ss.Delegate.create(this, function() {
        var userLanguageLoadFailed = ss.Delegate.create(this, function(error) {
            this.startStage2();
        });
        var userLanguageLoadFinished = ss.Delegate.create(this, function() {
            this.startStage2();
        });
        try {
            afung.MangaWeb3.Client.Strings.loadUserLanguage(userLanguageLoadFinished, userLanguageLoadFailed);
        }
        catch (error) {
            userLanguageLoadFailed(error);
        }
    });
    try {
        afung.MangaWeb3.Client.Strings.loadDefaultLanguage(defaultLanguageLoadFinished, defaultLanguageLoadFailed);
    }
    catch (error) {
        defaultLanguageLoadFailed(error);
    }
}
afung.MangaWeb3.Client.Application.showFatalError = function afung_MangaWeb3_Client_Application$showFatalError(message) {
    /// <summary>
    /// Show fatal error
    /// </summary>
    /// <param name="message" type="String">
    /// the message to be shown
    /// </param>
    var errorMessageObject = $('<div class="container"><div class="row"><div class="span12"><div class="alert alert-error"><h4>Fatal Error</h4><span class="fatalerrormsg"></span></div></div></div></div>');
    $('body').prepend(errorMessageObject);
    $('.fatalerrormsg', errorMessageObject).text(message);
}
afung.MangaWeb3.Client.Application.prototype = {
    
    startStage2: function afung_MangaWeb3_Client_Application$startStage2() {
        /// <summary>
        /// Stage 2 of booting up the application
        /// </summary>
        var templateLoadFailed = function(error) {
            afung.MangaWeb3.Client.Application.showFatalError(error.toString());
        };
        var templateLoadFinished = ss.Delegate.create(this, function() {
            this.loadFirstModule();
        });
        afung.MangaWeb3.Client.Template.loadTemplateFile(templateLoadFinished, templateLoadFailed);
    },
    
    loadFirstModule: function afung_MangaWeb3_Client_Application$loadFirstModule() {
    }
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.HtmlConstants

afung.MangaWeb3.Client.HtmlConstants = function afung_MangaWeb3_Client_HtmlConstants() {
    /// <summary>
    /// Class for constants related to HTML
    /// </summary>
    /// <field name="attributeChecked" type="String" static="true">
    /// The checked attribute
    /// </field>
    /// <field name="attributeDisabled" type="String" static="true">
    /// The disabled attribute
    /// </field>
    /// <field name="attributeId" type="String" static="true">
    /// The ID attribute
    /// </field>
    /// <field name="attributePlaceHolder" type="String" static="true">
    /// The Placeholder attribute
    /// </field>
    /// <field name="classTemp" type="String" static="true">
    /// The class temp
    /// </field>
    /// <field name="tagBody" type="String" static="true">
    /// Tag Body
    /// </field>
    /// <field name="tagDiv" type="String" static="true">
    /// Tag Div
    /// </field>
    /// <field name="tagParagraph" type="String" static="true">
    /// Tag Paragraph
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Strings

afung.MangaWeb3.Client.Strings = function afung_MangaWeb3_Client_Strings() {
    /// <summary>
    /// The Strings class which contains logic related to strings including language
    /// </summary>
    /// <field name="defaultLanguage" type="String" static="true">
    /// The default language is English (United States)
    /// </field>
    /// <field name="languages" type="Object" static="true">
    /// List of supported languages
    /// </field>
    /// <field name="_languageFilePathFormat" type="String" static="true">
    /// Format of language file path
    /// </field>
    /// <field name="_currentLanguage" type="String" static="true">
    /// Current Language
    /// </field>
    /// <field name="_loadedLanguageData" type="Object" static="true">
    /// Dictionary to store loaded language data
    /// </field>
}
afung.MangaWeb3.Client.Strings.loadDefaultLanguage = function afung_MangaWeb3_Client_Strings$loadDefaultLanguage(successCallback, failureCallback) {
    /// <param name="successCallback" type="Function">
    /// </param>
    /// <param name="failureCallback" type="System.Action`1">
    /// </param>
    afung.MangaWeb3.Client.Strings.loadLanguageFile('en-us', successCallback, failureCallback);
}
afung.MangaWeb3.Client.Strings.loadUserLanguage = function afung_MangaWeb3_Client_Strings$loadUserLanguage(successCallback, failureCallback) {
    /// <param name="successCallback" type="Function">
    /// </param>
    /// <param name="failureCallback" type="System.Action`1">
    /// </param>
    afung.MangaWeb3.Client.Strings.loadLanguageFile(afung.MangaWeb3.Client.Strings._currentLanguage, successCallback, failureCallback);
}
afung.MangaWeb3.Client.Strings.loadLanguageFile = function afung_MangaWeb3_Client_Strings$loadLanguageFile(language, successCallback, failureCallback) {
    /// <param name="language" type="String">
    /// </param>
    /// <param name="successCallback" type="Function">
    /// </param>
    /// <param name="failureCallback" type="System.Action`1">
    /// </param>
    if (Object.keyExists(afung.MangaWeb3.Client.Strings._loadedLanguageData, language)) {
        successCallback();
        return;
    }
    var onError = function(request, textStatus, error) {
        failureCallback(error);
    };
    var onFinish = function(data, textStatus, request) {
        afung.MangaWeb3.Client.Strings._loadedLanguageData[language] = {};
        $(data).each(function(index, element) {
            var currentObject = $(element);
            if (!String.compare(element.tagName, 'p', true)) {
                var stringId = currentObject.attr('id');
                if (!String.isNullOrEmpty(stringId)) {
                    afung.MangaWeb3.Client.Strings._loadedLanguageData[language][stringId] = currentObject.html();
                }
            }
        });
        successCallback();
    };
    var options = {};
    options.type = 'GET';
    options.dataType = 'html';
    options.error = onError;
    options.success = onFinish;
    $.ajax(String.format('lang/{0}.html', language), options);
}
afung.MangaWeb3.Client.Strings.getHtml = function afung_MangaWeb3_Client_Strings$getHtml(stringId) {
    /// <summary>
    /// Get the string from string ID
    /// </summary>
    /// <param name="stringId" type="String">
    /// String ID
    /// </param>
    /// <returns type="String"></returns>
    if (afung.MangaWeb3.Client.Strings._currentLanguage !== 'en-us' && Object.keyExists(afung.MangaWeb3.Client.Strings._loadedLanguageData, afung.MangaWeb3.Client.Strings._currentLanguage) && Object.keyExists(afung.MangaWeb3.Client.Strings._loadedLanguageData[afung.MangaWeb3.Client.Strings._currentLanguage], stringId)) {
        return afung.MangaWeb3.Client.Strings._loadedLanguageData[afung.MangaWeb3.Client.Strings._currentLanguage][stringId];
    }
    else if (Object.keyExists(afung.MangaWeb3.Client.Strings._loadedLanguageData, 'en-us') && Object.keyExists(afung.MangaWeb3.Client.Strings._loadedLanguageData['en-us'], stringId)) {
        return afung.MangaWeb3.Client.Strings._loadedLanguageData['en-us'][stringId];
    }
    return 'String Not Defined';
}
afung.MangaWeb3.Client.Strings.get = function afung_MangaWeb3_Client_Strings$get(stringId) {
    /// <param name="stringId" type="String">
    /// </param>
    /// <returns type="String"></returns>
    return $('<span>' + afung.MangaWeb3.Client.Strings.getHtml(stringId) + '</span>').text();
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Template

afung.MangaWeb3.Client.Template = function afung_MangaWeb3_Client_Template() {
    /// <summary>
    /// Template class. Manage all template loading and parsing
    /// </summary>
    /// <field name="_templateFilePathFormat" type="String" static="true">
    /// Format of template file path
    /// </field>
    /// <field name="templates" type="Array" elementType="String" static="true">
    /// The template files to load
    /// </field>
    /// <field name="templateIds" type="Object" static="true">
    /// The IDs in template files to parse
    /// The order of the IDs matters!
    /// </field>
    /// <field name="_tempDivObject" type="jQueryObject" static="true">
    /// </field>
    /// <field name="_loadedTemplateData" type="Object" static="true">
    /// The loaded template data
    /// </field>
}
afung.MangaWeb3.Client.Template.get__tempDivObject = function afung_MangaWeb3_Client_Template$get__tempDivObject() {
    /// <value type="jQueryObject"></value>
    if (afung.MangaWeb3.Client.Template._tempDivObject == null) {
        afung.MangaWeb3.Client.Template._tempDivObject = $(document.createElement('div')).appendTo($('body')).addClass('temp');
    }
    return afung.MangaWeb3.Client.Template._tempDivObject;
}
afung.MangaWeb3.Client.Template.loadTemplateFile = function afung_MangaWeb3_Client_Template$loadTemplateFile(successCallback, failureCallback) {
    /// <param name="successCallback" type="Function">
    /// </param>
    /// <param name="failureCallback" type="System.Action`1">
    /// </param>
    var templateIndex = 0;
    var currentTemplateFile = '';
    var loadNextTemplate = function() {
    };
    var onError = function(request, textStatus, error) {
        failureCallback(new Error(String.format(afung.MangaWeb3.Client.Strings.get('TemplateLoadError'), currentTemplateFile, error)));
    };
    var onFinish = function(data, textStatus, request) {
        var templateDiv = $(document.createElement('div')).appendTo(afung.MangaWeb3.Client.Template.get__tempDivObject()).append($(data));
        afung.MangaWeb3.Client.Template._loadedTemplateData[currentTemplateFile] = {};
        var $enum1 = ss.IEnumerator.getEnumerator(afung.MangaWeb3.Client.Template.templateIds[currentTemplateFile]);
        while ($enum1.moveNext()) {
            var templateId = $enum1.current;
            var selectedTemplate = $('#' + templateId, templateDiv);
            if (selectedTemplate.length !== 1) {
                failureCallback(new Error(String.format(afung.MangaWeb3.Client.Strings.get('TemplateParseError'), currentTemplateFile, templateId)));
                return;
            }
            afung.MangaWeb3.Client.Template._loadedTemplateData[currentTemplateFile][templateId] = selectedTemplate.clone();
            selectedTemplate.remove();
        }
        templateDiv.remove();
        templateIndex++;
        loadNextTemplate();
    };
    loadNextTemplate = function() {
        if (templateIndex >= afung.MangaWeb3.Client.Template.templates.length) {
            successCallback();
            return;
        }
        currentTemplateFile = afung.MangaWeb3.Client.Template.templates[templateIndex];
        var options = {};
        options.type = 'GET';
        options.dataType = 'html';
        options.error = onError;
        options.success = onFinish;
        $.ajax(String.format('template/{0}.html', currentTemplateFile), options);
    };
    loadNextTemplate();
}
afung.MangaWeb3.Client.Template.get = function afung_MangaWeb3_Client_Template$get(template, templateId) {
    /// <param name="template" type="String">
    /// </param>
    /// <param name="templateId" type="String">
    /// </param>
    /// <returns type="jQueryObject"></returns>
    if (Object.keyExists(afung.MangaWeb3.Client.Template._loadedTemplateData, template) && Object.keyExists(afung.MangaWeb3.Client.Template._loadedTemplateData[template], templateId)) {
        var obj = afung.MangaWeb3.Client.Template._loadedTemplateData[template][templateId];
        obj = obj.clone().appendTo(afung.MangaWeb3.Client.Template.get__tempDivObject());
        $('.msg', obj).each(function(index, element) {
            var msgObj = $(element);
            var $enum1 = ss.IEnumerator.getEnumerator(msgObj.attr('class').split(' '));
            while ($enum1.moveNext()) {
                var className = $enum1.current;
                if (className.startsWith('msg-')) {
                    msgObj.html(afung.MangaWeb3.Client.Strings.getHtml(className.substr(4)));
                    break;
                }
            }
        });
        $('.plhdr', obj).each(function(index, element) {
            var msgObj = $(element);
            var $enum1 = ss.IEnumerator.getEnumerator(msgObj.attr('class').split(' '));
            while ($enum1.moveNext()) {
                var className = $enum1.current;
                if (className.startsWith('plhdr-')) {
                    msgObj.attr('placeholder', afung.MangaWeb3.Client.Strings.getHtml(className.substr(6)));
                    break;
                }
            }
        });
        return obj.detach();
    }
    throw new Error(String.format(afung.MangaWeb3.Client.Strings.get('TemplateGetError'), template, templateId));
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Environment

afung.MangaWeb3.Client.Environment = function afung_MangaWeb3_Client_Environment() {
    /// <summary>
    /// Contain environment variables
    /// </summary>
    /// <field name="serverType" type="afung.MangaWeb3.Client.ServerType" static="true">
    /// The server type.
    /// Should be set from servertype.js
    /// </field>
}


Type.registerNamespace('afung.MangaWeb3.Client.Module');

////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Module.ModuleBase

afung.MangaWeb3.Client.Module.ModuleBase = function afung_MangaWeb3_Client_Module_ModuleBase(template, templateId) {
    /// <param name="template" type="String">
    /// </param>
    /// <param name="templateId" type="String">
    /// </param>
    /// <field name="_currentModule" type="afung.MangaWeb3.Client.Module.ModuleBase" static="true">
    /// </field>
    /// <field name="attachedObject" type="jQueryObject">
    /// </field>
    if (afung.MangaWeb3.Client.Module.ModuleBase._currentModule != null) {
        afung.MangaWeb3.Client.Module.ModuleBase._currentModule.dispose();
    }
    afung.MangaWeb3.Client.Module.ModuleBase._currentModule = null;
    afung.MangaWeb3.Client.Module.ModuleBase._currentModule = this;
    this.attachedObject = afung.MangaWeb3.Client.Template.get(template, templateId).appendTo($('body'));
    this.initialize();
}
afung.MangaWeb3.Client.Module.ModuleBase.prototype = {
    attachedObject: null,
    
    dispose: function afung_MangaWeb3_Client_Module_ModuleBase$dispose() {
        this.attachedObject.remove();
    }
}


afung.MangaWeb3.Client.Modal.ModalBase.registerClass('afung.MangaWeb3.Client.Modal.ModalBase');
afung.MangaWeb3.Client.Modal.ErrorModal.registerClass('afung.MangaWeb3.Client.Modal.ErrorModal', afung.MangaWeb3.Client.Modal.ModalBase);
afung.MangaWeb3.Client.Request.registerClass('afung.MangaWeb3.Client.Request');
afung.MangaWeb3.Client.Application.registerClass('afung.MangaWeb3.Client.Application');
afung.MangaWeb3.Client.HtmlConstants.registerClass('afung.MangaWeb3.Client.HtmlConstants');
afung.MangaWeb3.Client.Strings.registerClass('afung.MangaWeb3.Client.Strings');
afung.MangaWeb3.Client.Template.registerClass('afung.MangaWeb3.Client.Template');
afung.MangaWeb3.Client.Environment.registerClass('afung.MangaWeb3.Client.Environment');
afung.MangaWeb3.Client.Module.ModuleBase.registerClass('afung.MangaWeb3.Client.Module.ModuleBase', null, ss.IDisposable);
afung.MangaWeb3.Client.Modal.ErrorModal._instance$1 = null;
afung.MangaWeb3.Client.Request.endpoint = 'ServerAjax';
afung.MangaWeb3.Client.HtmlConstants.attributeChecked = 'checked';
afung.MangaWeb3.Client.HtmlConstants.attributeDisabled = 'disabled';
afung.MangaWeb3.Client.HtmlConstants.attributeId = 'id';
afung.MangaWeb3.Client.HtmlConstants.attributePlaceHolder = 'placeholder';
afung.MangaWeb3.Client.HtmlConstants.classTemp = 'temp';
afung.MangaWeb3.Client.HtmlConstants.tagBody = 'body';
afung.MangaWeb3.Client.HtmlConstants.tagDiv = 'div';
afung.MangaWeb3.Client.HtmlConstants.tagParagraph = 'p';
afung.MangaWeb3.Client.Strings.defaultLanguage = 'en-us';
afung.MangaWeb3.Client.Strings.languages = { 'en-us': 'English (United States)', 'zh-hk': 'Chinese (Hong Kong)' };
afung.MangaWeb3.Client.Strings._currentLanguage = 'en-us';
afung.MangaWeb3.Client.Strings._loadedLanguageData = {};
afung.MangaWeb3.Client.Template.templates = [ 'client' ];
afung.MangaWeb3.Client.Template.templateIds = { client: [ 'error-modal' ] };
afung.MangaWeb3.Client.Template._tempDivObject = null;
afung.MangaWeb3.Client.Template._loadedTemplateData = {};
afung.MangaWeb3.Client.Environment.serverType = 0;
afung.MangaWeb3.Client.Module.ModuleBase._currentModule = null;
})();

//! This script was generated using Script# v0.7.4.0
