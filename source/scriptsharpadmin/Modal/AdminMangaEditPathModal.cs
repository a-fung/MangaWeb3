// AdminMangaEditPathModal.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Admin.Module;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Modal
{
    public class AdminMangaEditPathModal : ModalBase
    {
        private static AdminMangaEditPathModal instance = null;

        private static AdminMangasModule mangasModule = null;

        private bool submittingForm = false;
        private int collectionId;
        private int mangaId;

        private AdminMangaEditPathModal()
            : base("admin", "admin-manga-edit-path-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#admin-manga-edit-path-submit").Click(SubmitForm);
            jQuery.Select("#admin-manga-edit-path-form").Submit(SubmitForm);
            jQuery.Select("#admin-manga-edit-path-browse-btn").Click(BrowseButtonClicked);
        }

        protected override void OnShown()
        {
            jQuery.Select("#admin-manga-edit-path").Focus();
        }

        public static void ShowDialog(AdminMangasModule mangasModule, int id)
        {
            if (instance == null)
            {
                instance = new AdminMangaEditPathModal();
            }

            instance.InternalShow(id);
            AdminMangaEditPathModal.mangasModule = mangasModule;
        }

        public void InternalShow(int id)
        {
            AdminMangaEditPathRequest request = new AdminMangaEditPathRequest();
            mangaId = request.id = id;

            Request.Send(request, GetMangaSuccess);
        }

        [AlternateSignature]
        private extern void GetMangaSuccess(JsonResponse response);
        private void GetMangaSuccess(AdminMangaEditPathResponse response)
        {
            Show();
            jQuery.Select("#admin-manga-edit-path").Value(response.path).Focus();
            collectionId = response.cid;
        }

        private void SubmitForm(jQueryEvent e)
        {
            e.PreventDefault();
            string path = jQuery.Select("#admin-manga-edit-path").GetValue();
            if (path != "" && !submittingForm)
            {
                AdminMangaEditPathRequest request = new AdminMangaEditPathRequest();
                request.id = mangaId;
                request.path = path;
                submittingForm = true;

                Request.Send(request, SubmitSuccess, SubmitFailure);
            }
        }

        [AlternateSignature]
        private extern void SubmitSuccess(JsonResponse response);
        private void SubmitSuccess(AdminMangaEditPathResponse response)
        {
            submittingForm = false;

            switch (response.status)
            {
                case 0:
                    Hide();
                    mangasModule.Refresh();
                    break;
                case 1:
                    ErrorModal.ShowError(Strings.Get("FileNotFoundOrAlreadyAdded"));
                    break;
                case 3:
                    ErrorModal.ShowError(Strings.Get("InvalidFileType"));
                    break;
                default:
                    ErrorModal.ShowError(Strings.Get("UnknownError"));
                    break;
            }
        }

        private void SubmitFailure(Exception error)
        {
            submittingForm = false;

            ErrorModal.ShowError(error.ToString());
        }

        private void BrowseButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (collectionId > 0)
            {
                AdminFinderModal.ShowDialog(jQuery.Select("#admin-manga-edit-path"), collectionId);
            }
        }
    }
}
