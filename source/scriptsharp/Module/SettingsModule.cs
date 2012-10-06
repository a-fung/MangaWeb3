// SettingsModule.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Client.Module
{
    public class SettingsModule : ClientModuleBase
    {
        private static SettingsModule _instance = null;
        public static SettingsModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsModule();
                }

                return _instance;
            }
        }

        private SettingsModule()
            : base("settings-module")
        {
        }

        protected override void OnShow()
        {
        }
    }
}
