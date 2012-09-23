using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminSettingsGetRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminSettingsGetRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminSettingsGetResponse response = new AdminSettingsGetResponse();
            response.guest = Settings.AllowGuest;
            response.zip = Settings.UseZip;
            response.rar = Settings.UseRar;
            response.pdf = Settings.UsePdf;

            ajax.ReturnJson(response);
        }
    }
}