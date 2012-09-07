//! afung.MangaWeb3.Client.debug.js
//

(function() {

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
            afung.MangaWeb3.Client.Application.showFatalError(String.format('Unabled to load template file ({0}). Please check that you have the correct file on your server', error));
        };
        var templateLoadFinished = function() {
        };
        afung.MangaWeb3.Client.Template.loadTemplateFile(templateLoadFinished, templateLoadFailed);
    }
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.HtmlConstants

afung.MangaWeb3.Client.HtmlConstants = function afung_MangaWeb3_Client_HtmlConstants() {
    /// <summary>
    /// Class for constants related to HTML
    /// </summary>
    /// <field name="attributeId" type="String" static="true">
    /// The ID attribute
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
    afung.MangaWeb3.Client.Strings.loadLanguageFile('en-us', successCallback, failureCallback);
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
afung.MangaWeb3.Client.Strings.get = function afung_MangaWeb3_Client_Strings$get(stringId) {
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
    /// <field name="_loadedTemplateData" type="Object" static="true">
    /// The loaded template data
    /// </field>
}
afung.MangaWeb3.Client.Template.loadTemplateFile = function afung_MangaWeb3_Client_Template$loadTemplateFile(successCallback, failureCallback) {
    /// <param name="successCallback" type="Function">
    /// </param>
    /// <param name="failureCallback" type="System.Action`1">
    /// </param>
    var templateIndex = 0;
    var currentTemplateFile = '';
    var tempDiv = $(document.createElement('div')).appendTo($('body')).addClass('temp');
    var loadNextTemplate = function() {
    };
    var onError = function(request, textStatus, error) {
        failureCallback(error);
    };
    var onFinish = function(data, textStatus, request) {
        var templateDiv = $(document.createElement('div')).appendTo(tempDiv).append($(data));
        afung.MangaWeb3.Client.Template._loadedTemplateData[currentTemplateFile] = {};
        var $enum1 = ss.IEnumerator.getEnumerator(afung.MangaWeb3.Client.Template.templateIds[currentTemplateFile]);
        while ($enum1.moveNext()) {
            var templateId = $enum1.current;
            var selectedTemplate = $('#' + templateId, templateDiv);
            afung.MangaWeb3.Client.Template._loadedTemplateData[currentTemplateFile][templateId] = selectedTemplate.clone();
            selectedTemplate.remove();
        }
        templateDiv.remove();
        templateIndex++;
        loadNextTemplate();
    };
    loadNextTemplate = function() {
        if (templateIndex >= afung.MangaWeb3.Client.Template.templates.length) {
            tempDiv.remove();
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


afung.MangaWeb3.Client.Application.registerClass('afung.MangaWeb3.Client.Application');
afung.MangaWeb3.Client.HtmlConstants.registerClass('afung.MangaWeb3.Client.HtmlConstants');
afung.MangaWeb3.Client.Strings.registerClass('afung.MangaWeb3.Client.Strings');
afung.MangaWeb3.Client.Template.registerClass('afung.MangaWeb3.Client.Template');
afung.MangaWeb3.Client.Environment.registerClass('afung.MangaWeb3.Client.Environment');
afung.MangaWeb3.Client.HtmlConstants.attributeId = 'id';
afung.MangaWeb3.Client.HtmlConstants.classTemp = 'temp';
afung.MangaWeb3.Client.HtmlConstants.tagBody = 'body';
afung.MangaWeb3.Client.HtmlConstants.tagDiv = 'div';
afung.MangaWeb3.Client.HtmlConstants.tagParagraph = 'p';
afung.MangaWeb3.Client.Strings.defaultLanguage = 'en-us';
afung.MangaWeb3.Client.Strings.languages = { 'en-us': 'English (United States)', 'zh-hk': 'Chinese (Hong Kong)' };
afung.MangaWeb3.Client.Strings._currentLanguage = 'en-us';
afung.MangaWeb3.Client.Strings._loadedLanguageData = {};
afung.MangaWeb3.Client.Template.templates = [ 'client' ];
afung.MangaWeb3.Client.Template.templateIds = { client: [ 'div3', 'div2', 'div1' ] };
afung.MangaWeb3.Client.Template._loadedTemplateData = {};
afung.MangaWeb3.Client.Environment.serverType = 0;
})();

//! This script was generated using Script# v0.7.4.0
