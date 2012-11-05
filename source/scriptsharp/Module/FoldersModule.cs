// FoldersModule.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Widget;
using afung.MangaWeb3.Common;
using jQueryApi;

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
                    Action loginRefreshCallback = delegate
                    {
                        if (_instance != null)
                        {
                            _instance.Refresh();
                        }
                    };

                    _instance = new FoldersModule(loginRefreshCallback);
                }

                return _instance;
            }
        }

        private FoldersModule(Action loginRefreshCallback)
            : base("folders-module", loginRefreshCallback)
        {
            jQuery.Select("#folders-all-folders-btn").Click(AllFoldersButtonClicked);
            Refresh();
        }

        public void Refresh()
        {
            jQuery.Select("#folders-area").Children().Remove();
            Request.Send(new FolderRequest(), FolderRequestSuccess);
            Template.Get("client", "loading-well", true).AppendTo(jQuery.Select("#folders-area"));
        }

        protected override void OnShow()
        {
        }

        private void AllFoldersButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            MangasModule.Instance.Refresh(new MangaFilter());
        }

        [AlternateSignature]
        private extern void FolderRequestSuccess(JsonResponse response);
        private void FolderRequestSuccess(FolderResponse response)
        {
            if (response.status == 0)
            {
                jQuery.Select("#folders-area").Children().Remove();
                if (response.folders.Length == 0)
                {
                    Template.Get("client", "noitem-well", true).AppendTo(jQuery.Select("#folders-area"));
                }

                new FoldersWidget(jQuery.Select("#folders-area"), response.folders, "");
            }
            else
            {
                Window.SetTimeout(
                    delegate
                    {
                        Request.Send(new FolderRequest(), FolderRequestSuccess);
                    },
                    1000);
            }
        }
    }
}
