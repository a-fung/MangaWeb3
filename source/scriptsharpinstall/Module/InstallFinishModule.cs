// InstallFinishModule.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Module;

namespace afung.MangaWeb3.Client.Install.Module
{
    public class InstallFinishModule : ModuleBase
    {
        private static InstallFinishModule _instance = null;
        public static InstallFinishModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new InstallFinishModule();
                }

                return _instance;
            }
        }

        private InstallFinishModule()
            : base("install", "install-finish-module")
        {
        }

        protected override void OnShow()
        {
        }
    }
}
