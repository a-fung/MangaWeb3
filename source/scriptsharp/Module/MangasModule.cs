// MangasModule.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Client.Module
{
    public class MangasModule : ClientModuleBase
    {
        private static MangasModule _instance = null;
        public static MangasModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MangasModule();
                }

                return _instance;
            }
        }

        private MangasModule()
            : base("mangas-module")
        {
        }

        protected override void OnShow()
        {
        }
    }
}
