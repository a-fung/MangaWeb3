// SearchModule.cs
//

using System;
using System.Collections.Generic;

namespace afung.MangaWeb3.Client.Module
{
    public class SearchModule : ClientModuleBase
    {
        private static SearchModule _instance = null;
        public static SearchModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SearchModule();
                }

                return _instance;
            }
        }

        private SearchModule()
            : base("search-module")
        {
        }

        protected override void OnShow()
        {
        }
    }
}
