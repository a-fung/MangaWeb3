﻿using System;
using System.Collections.Generic;
using System.Linq;
using afung.MangaWeb3.Server.Handler;
using afung.MangaWeb3.Server.Install.Handler;

namespace afung.MangaWeb3.Server.Install
{
    public partial class InstallAjax : AjaxBase
    {
        private static HandlerBase[] handlers = null;

        protected override void PageLoad()
        {
            if (Config.IsInstalled)
            {
                BadRequest();
                return;
            }

            if (handlers == null)
            {
                handlers = new HandlerBase[]{
                    new PreInstallCheckRequestHandler(),
                    new CheckMySqlSettingRequestHandler(),
                    new CheckOtherComponentRequestHandler(),
                    new InstallRequestHandler(),
                };
            }

            HandleRequest(handlers);
        }
    }
}