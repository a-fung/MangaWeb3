//! afung.MangaWeb3.Client.debug.js
//

(function() {

Type.registerNamespace('afung.MangaWeb3.Client');

////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.ServerType

afung.MangaWeb3.Client.ServerType = function() { 
    /// <field name="aspNet" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="php" type="Number" integer="true" static="true">
    /// </field>
};
afung.MangaWeb3.Client.ServerType.prototype = {
    aspNet: 0, 
    php: 1
}
afung.MangaWeb3.Client.ServerType.registerEnum('afung.MangaWeb3.Client.ServerType', false);


////////////////////////////////////////////////////////////////////////////////
// afung.MangaWeb3.Client.Environment

afung.MangaWeb3.Client.Environment = function afung_MangaWeb3_Client_Environment() {
    /// <field name="serverType" type="afung.MangaWeb3.Client.ServerType" static="true">
    /// </field>
}


afung.MangaWeb3.Client.Environment.registerClass('afung.MangaWeb3.Client.Environment');
afung.MangaWeb3.Client.Environment.serverType = 0;
})();

//! This script was generated using Script# v0.7.4.0
