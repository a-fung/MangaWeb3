//! afung.MangaWeb3.Common.debug.js
//

(function() {

Type.registerNamespace('afung.MangaWeb3.Common');

////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Common.CheckMySqlSettingRequest

afung.MangaWeb3.Common.CheckMySqlSettingRequest = function afung_MangaWeb3_Common_CheckMySqlSettingRequest() {
    /// <field name="server" type="String">
    /// </field>
    /// <field name="port" type="Number" integer="true">
    /// </field>
    /// <field name="username" type="String">
    /// </field>
    /// <field name="password" type="String">
    /// </field>
    /// <field name="database" type="String">
    /// </field>
    afung.MangaWeb3.Common.CheckMySqlSettingRequest.initializeBase(this);
}
afung.MangaWeb3.Common.CheckMySqlSettingRequest.prototype = {
    server: null,
    port: 0,
    username: null,
    password: null,
    database: null
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Common.CheckMySqlSettingResponse

afung.MangaWeb3.Common.CheckMySqlSettingResponse = function afung_MangaWeb3_Common_CheckMySqlSettingResponse() {
    /// <field name="pass" type="Boolean">
    /// </field>
    afung.MangaWeb3.Common.CheckMySqlSettingResponse.initializeBase(this);
}
afung.MangaWeb3.Common.CheckMySqlSettingResponse.prototype = {
    pass: false
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Common.ErrorResponse

afung.MangaWeb3.Common.ErrorResponse = function afung_MangaWeb3_Common_ErrorResponse() {
    /// <field name="errorCode" type="Number" integer="true">
    /// </field>
    /// <field name="errorMsg" type="String">
    /// </field>
    afung.MangaWeb3.Common.ErrorResponse.initializeBase(this);
}
afung.MangaWeb3.Common.ErrorResponse.prototype = {
    errorCode: 0,
    errorMsg: null
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Common.JsonRequest

afung.MangaWeb3.Common.JsonRequest = function afung_MangaWeb3_Common_JsonRequest() {
    /// <field name="type" type="String">
    /// </field>
    this.type = Type.getInstanceType(this).get_name();
}
afung.MangaWeb3.Common.JsonRequest.prototype = {
    type: null
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Common.JsonResponse

afung.MangaWeb3.Common.JsonResponse = function afung_MangaWeb3_Common_JsonResponse() {
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Common.PreInstallCheckRequest

afung.MangaWeb3.Common.PreInstallCheckRequest = function afung_MangaWeb3_Common_PreInstallCheckRequest() {
    afung.MangaWeb3.Common.PreInstallCheckRequest.initializeBase(this);
}


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Common.PreInstallCheckResponse

afung.MangaWeb3.Common.PreInstallCheckResponse = function afung_MangaWeb3_Common_PreInstallCheckResponse() {
    /// <field name="installed" type="Boolean">
    /// </field>
    /// <field name="mySql" type="Boolean">
    /// </field>
    /// <field name="gd" type="Boolean">
    /// </field>
    /// <field name="zip" type="Boolean">
    /// </field>
    /// <field name="rar" type="Boolean">
    /// </field>
    /// <field name="pdfinfo" type="Boolean">
    /// </field>
    /// <field name="pdfdraw" type="Boolean">
    /// </field>
    afung.MangaWeb3.Common.PreInstallCheckResponse.initializeBase(this);
}
afung.MangaWeb3.Common.PreInstallCheckResponse.prototype = {
    installed: false,
    mySql: false,
    gd: false,
    zip: false,
    rar: false,
    pdfinfo: false,
    pdfdraw: false
}


afung.MangaWeb3.Common.JsonRequest.registerClass('afung.MangaWeb3.Common.JsonRequest');
afung.MangaWeb3.Common.CheckMySqlSettingRequest.registerClass('afung.MangaWeb3.Common.CheckMySqlSettingRequest', afung.MangaWeb3.Common.JsonRequest);
afung.MangaWeb3.Common.JsonResponse.registerClass('afung.MangaWeb3.Common.JsonResponse');
afung.MangaWeb3.Common.CheckMySqlSettingResponse.registerClass('afung.MangaWeb3.Common.CheckMySqlSettingResponse', afung.MangaWeb3.Common.JsonResponse);
afung.MangaWeb3.Common.ErrorResponse.registerClass('afung.MangaWeb3.Common.ErrorResponse', afung.MangaWeb3.Common.JsonResponse);
afung.MangaWeb3.Common.PreInstallCheckRequest.registerClass('afung.MangaWeb3.Common.PreInstallCheckRequest', afung.MangaWeb3.Common.JsonRequest);
afung.MangaWeb3.Common.PreInstallCheckResponse.registerClass('afung.MangaWeb3.Common.PreInstallCheckResponse', afung.MangaWeb3.Common.JsonResponse);
})();

//! This script was generated using Script# v0.7.4.0
