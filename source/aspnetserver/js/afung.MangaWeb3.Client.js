// afung.MangaWeb3.Client.js
(function(){
Type.registerNamespace('afung.MangaWeb3.Client');afung.MangaWeb3.Client.ServerType=function(){};afung.MangaWeb3.Client.ServerType.prototype = {aspNet:0,php:1}
afung.MangaWeb3.Client.ServerType.registerEnum('afung.MangaWeb3.Client.ServerType',false);afung.MangaWeb3.Client.Application=function(){var $0=function($p1_0){
afung.MangaWeb3.Client.Application.showFatalError(String.format('Unabled to load default language file ({0}). Please check that you have the correct file on your server',$p1_0));};var $1=ss.Delegate.create(this,function(){
var $1_0=ss.Delegate.create(this,function($p2_0){
this.startStage2();});var $1_1=ss.Delegate.create(this,function(){
this.startStage2();});try{afung.MangaWeb3.Client.Strings.loadUserLanguage($1_1,$1_0);}catch($1_2){$1_0($1_2);}});try{afung.MangaWeb3.Client.Strings.loadDefaultLanguage($1,$0);}catch($2){$0($2);}}
afung.MangaWeb3.Client.Application.showFatalError=function(message){var $0=$('<div class="container"><div class="row"><div class="span12"><div class="alert alert-error"><h4>Fatal Error</h4><span class="fatalerrormsg"></span></div></div></div></div>');$('body').prepend($0);$('.fatalerrormsg',$0).text(message);}
afung.MangaWeb3.Client.Application.prototype={startStage2:function(){var $0=function($p1_0){
afung.MangaWeb3.Client.Application.showFatalError(String.format('Unabled to load template file ({0}). Please check that you have the correct file on your server',$p1_0));};var $1=function(){
};afung.MangaWeb3.Client.Template.loadTemplateFile($1,$0);}}
afung.MangaWeb3.Client.HtmlConstants=function(){}
afung.MangaWeb3.Client.Strings=function(){}
afung.MangaWeb3.Client.Strings.loadDefaultLanguage=function(successCallback,failureCallback){afung.MangaWeb3.Client.Strings.loadLanguageFile('en-us',successCallback,failureCallback);}
afung.MangaWeb3.Client.Strings.loadUserLanguage=function(successCallback,failureCallback){afung.MangaWeb3.Client.Strings.loadLanguageFile('en-us',successCallback,failureCallback);}
afung.MangaWeb3.Client.Strings.loadLanguageFile=function(language,successCallback,failureCallback){if(Object.keyExists(afung.MangaWeb3.Client.Strings.$2,language)){successCallback();return;}var $0=function($p1_0,$p1_1,$p1_2){
failureCallback($p1_2);};var $1=function($p1_0,$p1_1,$p1_2){
afung.MangaWeb3.Client.Strings.$2[language]={};$($p1_0).each(function($p2_0,$p2_1){
var $2_0=$($p2_1);if(!String.compare($p2_1.tagName,'p',true)){var $2_1=$2_0.attr('id');if(!String.isNullOrEmpty($2_1)){afung.MangaWeb3.Client.Strings.$2[language][$2_1]=$2_0.html();}}});successCallback();};var $2={};$2.type='GET';$2.dataType='html';$2.error=$0;$2.success=$1;$.ajax(String.format('lang/{0}.html',language),$2);}
afung.MangaWeb3.Client.Strings.get=function(stringId){if(afung.MangaWeb3.Client.Strings.$1!=='en-us'&&Object.keyExists(afung.MangaWeb3.Client.Strings.$2,afung.MangaWeb3.Client.Strings.$1)&&Object.keyExists(afung.MangaWeb3.Client.Strings.$2[afung.MangaWeb3.Client.Strings.$1],stringId)){return afung.MangaWeb3.Client.Strings.$2[afung.MangaWeb3.Client.Strings.$1][stringId];}else if(Object.keyExists(afung.MangaWeb3.Client.Strings.$2,'en-us')&&Object.keyExists(afung.MangaWeb3.Client.Strings.$2['en-us'],stringId)){return afung.MangaWeb3.Client.Strings.$2['en-us'][stringId];}return 'String Not Defined';}
afung.MangaWeb3.Client.Template=function(){}
afung.MangaWeb3.Client.Template.loadTemplateFile=function(successCallback,failureCallback){var $0=0;var $1='';var $2=$(document.createElement('div')).appendTo($('body')).addClass('temp');var $3=function(){
};var $4=function($p1_0,$p1_1,$p1_2){
failureCallback($p1_2);};var $5=function($p1_0,$p1_1,$p1_2){
var $1_0=$(document.createElement('div')).appendTo($2).append($($p1_0));afung.MangaWeb3.Client.Template.$1[$1]={};var $enum1=ss.IEnumerator.getEnumerator(afung.MangaWeb3.Client.Template.templateIds[$1]);while($enum1.moveNext()){var $1_1=$enum1.current;var $1_2=$('#'+$1_1,$1_0);afung.MangaWeb3.Client.Template.$1[$1][$1_1]=$1_2.clone();$1_2.remove();}$1_0.remove();$0++;$3();};$3=function(){
if($0>=afung.MangaWeb3.Client.Template.templates.length){$2.remove();successCallback();return;}$1=afung.MangaWeb3.Client.Template.templates[$0];var $1_0={};$1_0.type='GET';$1_0.dataType='html';$1_0.error=$4;$1_0.success=$5;$.ajax(String.format('template/{0}.html',$1),$1_0);};$3();}
afung.MangaWeb3.Client.Environment=function(){}
afung.MangaWeb3.Client.Application.registerClass('afung.MangaWeb3.Client.Application');afung.MangaWeb3.Client.HtmlConstants.registerClass('afung.MangaWeb3.Client.HtmlConstants');afung.MangaWeb3.Client.Strings.registerClass('afung.MangaWeb3.Client.Strings');afung.MangaWeb3.Client.Template.registerClass('afung.MangaWeb3.Client.Template');afung.MangaWeb3.Client.Environment.registerClass('afung.MangaWeb3.Client.Environment');afung.MangaWeb3.Client.HtmlConstants.attributeId='id';afung.MangaWeb3.Client.HtmlConstants.classTemp='temp';afung.MangaWeb3.Client.HtmlConstants.tagBody='body';afung.MangaWeb3.Client.HtmlConstants.tagDiv='div';afung.MangaWeb3.Client.HtmlConstants.tagParagraph='p';afung.MangaWeb3.Client.Strings.defaultLanguage='en-us';afung.MangaWeb3.Client.Strings.languages={'en-us':'English (United States)','zh-hk':'Chinese (Hong Kong)'};afung.MangaWeb3.Client.Strings.$1='en-us';afung.MangaWeb3.Client.Strings.$2={};afung.MangaWeb3.Client.Template.templates=['client'];afung.MangaWeb3.Client.Template.templateIds={client:['div3','div2','div1']};afung.MangaWeb3.Client.Template.$1={};afung.MangaWeb3.Client.Environment.serverType=0;})();// This script was generated using Script# v0.7.4.0
