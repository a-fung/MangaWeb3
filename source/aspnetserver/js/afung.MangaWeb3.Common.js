// afung.MangaWeb3.Common.js
(function(){
Type.registerNamespace('afung.MangaWeb3.Common');afung.MangaWeb3.Common.CheckMySqlSettingRequest=function(){afung.MangaWeb3.Common.CheckMySqlSettingRequest.initializeBase(this);}
afung.MangaWeb3.Common.CheckMySqlSettingRequest.prototype={server:null,port:0,username:null,password:null,database:null}
afung.MangaWeb3.Common.CheckMySqlSettingResponse=function(){afung.MangaWeb3.Common.CheckMySqlSettingResponse.initializeBase(this);}
afung.MangaWeb3.Common.CheckMySqlSettingResponse.prototype={pass:false}
afung.MangaWeb3.Common.ErrorResponse=function(){afung.MangaWeb3.Common.ErrorResponse.initializeBase(this);}
afung.MangaWeb3.Common.ErrorResponse.prototype={errorCode:0,errorMsg:null}
afung.MangaWeb3.Common.JsonRequest=function(){this.type=Type.getInstanceType(this).get_name();}
afung.MangaWeb3.Common.JsonRequest.prototype={type:null}
afung.MangaWeb3.Common.JsonResponse=function(){}
afung.MangaWeb3.Common.PreInstallCheckRequest=function(){afung.MangaWeb3.Common.PreInstallCheckRequest.initializeBase(this);}
afung.MangaWeb3.Common.PreInstallCheckResponse=function(){afung.MangaWeb3.Common.PreInstallCheckResponse.initializeBase(this);}
afung.MangaWeb3.Common.PreInstallCheckResponse.prototype={installed:false,mySql:false,gd:false,zip:false,rar:false,pdfinfo:false,pdfdraw:false}
afung.MangaWeb3.Common.JsonRequest.registerClass('afung.MangaWeb3.Common.JsonRequest');afung.MangaWeb3.Common.CheckMySqlSettingRequest.registerClass('afung.MangaWeb3.Common.CheckMySqlSettingRequest',afung.MangaWeb3.Common.JsonRequest);afung.MangaWeb3.Common.JsonResponse.registerClass('afung.MangaWeb3.Common.JsonResponse');afung.MangaWeb3.Common.CheckMySqlSettingResponse.registerClass('afung.MangaWeb3.Common.CheckMySqlSettingResponse',afung.MangaWeb3.Common.JsonResponse);afung.MangaWeb3.Common.ErrorResponse.registerClass('afung.MangaWeb3.Common.ErrorResponse',afung.MangaWeb3.Common.JsonResponse);afung.MangaWeb3.Common.PreInstallCheckRequest.registerClass('afung.MangaWeb3.Common.PreInstallCheckRequest',afung.MangaWeb3.Common.JsonRequest);afung.MangaWeb3.Common.PreInstallCheckResponse.registerClass('afung.MangaWeb3.Common.PreInstallCheckResponse',afung.MangaWeb3.Common.JsonResponse);})();// This script was generated using Script# v0.7.4.0
