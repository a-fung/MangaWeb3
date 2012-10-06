// FoldersModule.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Client.Module
{
    public class FoldersModule : ClientModuleBase
    {
        private static FoldersModule _instance = null;
        public static FoldersModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FoldersModule();
                }

                return _instance;
            }
        }

        private FoldersModule()
            : base("folders-module")
        {
        }

        protected override void OnShow()
        {
        }
    }
}
