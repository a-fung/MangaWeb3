// AdminFinderModal.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Common;
using Finder;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Modal
{
    public class AdminFinderModal : ModalBase
    {
        private static AdminFinderModal instance = null;

        private jQueryObject targetField = null;

        private int cid = -1;

        private CKFinder finder;

        private AdminFinderModal()
            : base("admin", "admin-finder-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#admin-finder-select").Click(SelectButtonClicked);
        }

        public static void ShowDialog(jQueryObject targetField, int cid)
        {
            if (!Script.IsNullOrUndefined(CKFinderWindow.CKFinder))
            {
                if (instance == null)
                {
                    instance = new AdminFinderModal();
                }

                instance.InternalShow(targetField, cid);
            }
        }

        public void InternalShow(jQueryObject targetField, int cid)
        {
            this.targetField = targetField;
            this.cid = cid;

            Action getToken = delegate
            {
                finder = null;
                jQuery.Select("#admin-finder-body").Children().Remove();

                AdminFinderRequest request = new AdminFinderRequest();
                request.cid = cid;
                Request.Send(request, FinderRequestSuccess);

                Show();
            };

            if (finder != null)
            {
                finder.Api.Destroy(getToken);
            }
            else
            {
                getToken();
            }
        }

        [AlternateSignature]
        private extern void FinderRequestSuccess(JsonResponse response);
        private void FinderRequestSuccess(AdminFinderResponse response)
        {
            CKFinder.Config.Language = Strings.Get("CKFinderLanguage");
            CKFinder.Config.ConnectorInfo = "token=" + response.token;

            finder = new CKFinder();
            finder.BasePath = "ckfinder/";
            finder.Callback = delegate(CKFinderAPI api)
            {
                api.DisableFileContextMenuOption("selectFile", false);
                api.DisableFileContextMenuOption("viewFile", false);
                api.DisableFileContextMenuOption("downloadFile", false);
                api.DisableFileContextMenuOption("renameFile", false);
                api.DisableFileContextMenuOption("deleteFile", false);
                api.DisableFolderContextMenuOption("removeFolder", false);
                api.DisableFolderContextMenuOption("kl", false);
                api.DisableFolderContextMenuOption("lI", false);

                jQueryObject finderFrame = jQuery.Select("#admin-finder-body iframe").Contents();
                finderFrame.Find("a.cke_button_upload").Remove();
                finderFrame.Find("a.cke_button_maximize").Remove();

                Show();
            };

            finder.SelectActionFunction = delegate(string fileUrl)
            {
                if (cid != -1)
                {
                    ChangeTargetFieldAndHide(fileUrl);
                }
            };

            finder.AppendTo("admin-finder-body");
        }

        private void SelectButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (cid == -1)
            {
                ChangeTargetFieldAndHide(finder.Api.GetSelectedFolder().GetUrl());
            }
            else
            {
                CKFinderFile file = finder.Api.GetSelectedFile();
                if (file != null)
                {
                    ChangeTargetFieldAndHide(file.GetUrl());
                }
            }
        }

        private void ChangeTargetFieldAndHide(string url)
        {
            url = url.DecodeUriComponent();

            if (Environment.ServerType == ServerType.AspNet)
            {
                url = url.Replace("/", @"\");
                while (url.IndexOf(@"\\") != -1)
                {
                    url = url.Replace(@"\\", @"\");
                }
            }
            else
            {
                url = url.Replace(@"\", "/");
                while (url.IndexOf("//") != -1)
                {
                    url = url.Replace("//", "/");
                }
            }

            targetField.Value(url);
            Hide();
        }
    }
}
