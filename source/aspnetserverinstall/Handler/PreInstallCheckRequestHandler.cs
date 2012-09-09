using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;
using afung.MangaWeb3.Server.Handler;

namespace afung.MangaWeb3.Server.Install.Handler
{
    public class PreInstallCheckRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(PreInstallCheckRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            PreInstallCheckResponse response = new PreInstallCheckResponse();
            response.installed = Config.IsInstalled;

            // assuming all these are good in asp.net version
            response.mySql = response.gd = response.zip = response.rar = response.pdfinfo = response.pdfdraw = true;

            ajax.ReturnJson(response);
        }
    }
}