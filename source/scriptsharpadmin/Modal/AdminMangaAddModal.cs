// AdminMangaAddModal.cs
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
    public class AdminMangaAddModal : ModalBase
    {
        private static AdminMangaAddModal instance = null;

        private static AdminMangasModule mangasModule = null;

        private bool submittingForm = false;

        private AdminMangaAddModal()
            : base("admin", "admin-manga-add-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#admin-manga-add-submit").Click(SubmitForm);
            jQuery.Select("#admin-manga-add-form").Submit(SubmitForm);
            jQuery.Select("#admin-manga-add-browse-btn").Click(BrowseButtonClicked);
        }

        protected override void OnShown()
        {
            jQuery.Select("#admin-manga-add-path").Focus();
        }

        public static void ShowDialog(AdminMangasModule mangasModule)
        {
            if (instance == null)
            {
                instance = new AdminMangaAddModal();
            }

            instance.InternalShow();
            AdminMangaAddModal.mangasModule = mangasModule;
        }

        public void InternalShow()
        {
            Show();

            jQuery.Select("#admin-manga-add-path").Value("").Focus();
            jQuery.Select("#admin-manga-add-collection").Children().Remove();
            Request.Send(new AdminCollectionsGetRequest(), GetRequestSuccess);
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminCollectionsGetResponse response)
        {
            foreach (CollectionJson collection in response.collections)
            {
                jQuery.FromHtml("<option></option>").AppendTo(jQuery.Select("#admin-manga-add-collection")).Value(collection.id.ToString()).Text(collection.name);
            }
        }

        private void SubmitForm(jQueryEvent e)
        {
            e.PreventDefault();

            string cidString = jQuery.Select("#admin-manga-add-collection").GetValue();
            string path = jQuery.Select("#admin-manga-add-path").GetValue();

            if (path == null || path == "" || cidString == null || cidString == "" || submittingForm)
            {
                return;
            }

            submittingForm = true;

            AdminMangaAddRequest request = new AdminMangaAddRequest();
            request.path = path;
            request.cid = int.Parse(cidString, 10);

            Request.Send(request, SubmitSuccess, SubmitFailure);
        }

        [AlternateSignature]
        private extern void SubmitSuccess(JsonResponse response);
        private void SubmitSuccess(AdminMangaAddResponse response)
        {
            submittingForm = false;

            switch (response.status)
            {
                case 0:
                    Hide();
                    // mangasModule.Refresh();
                    break;
                case 1:
                    ErrorModal.ShowError(Strings.Get("FileNotExist"));
                    break;
                case 2:
                    ErrorModal.ShowError(Strings.Get("PathNotUnderCollection"));
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

            string cidString = jQuery.Select("#admin-manga-add-collection").GetValue();

            if (cidString == null || cidString == "" || submittingForm)
            {
                return;
            }

            AdminFinderModal.ShowDialog(jQuery.Select("#admin-manga-add-path"), int.Parse(cidString, 10));
        }
    }
}
