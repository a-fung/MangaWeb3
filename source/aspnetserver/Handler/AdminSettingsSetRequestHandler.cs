using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminSettingsSetRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminSettingsSetRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminSettingsSetRequest request = Utility.ParseJson<AdminSettingsSetRequest>(jsonString);

            Settings.AllowGuest = request.guest;
            Settings.UseZip = request.zip;
            Settings.UseRar = request.rar;
            Settings.UsePdf = request.pdf;

            if (request.preprocessCount >= 0 && request.preprocessCount < 100)
            {
                Settings.MangaPagePreProcessCount = request.preprocessCount;
            }

            if (request.preprocessDelay >= 0 && request.preprocessDelay < 100000)
            {
                Settings.MangaPagePreProcessDelay = request.preprocessDelay;
            }

            if (request.cacheLimit >= 50 && request.cacheLimit < 100000)
            {
                Settings.MangaCacheSizeLimit = request.cacheLimit;
            }

            ajax.ReturnJson(new JsonResponse());
        }
    }
}