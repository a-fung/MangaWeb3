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
        afung.MangaWeb3.Client.Template.templateIds['install'] = [ 'install-mysql-error', 'install-module' ];
        afung.MangaWeb3.Client.Install.InstallApp.callBaseMethod(this, 'startStage2');
    }
}


afung.MangaWeb3.Client.Install.InstallApp.registerClass('afung.MangaWeb3.Client.Install.InstallApp', afung.MangaWeb3.Client.Application);
})();

//! This script was generated using Script# v0.7.4.0
