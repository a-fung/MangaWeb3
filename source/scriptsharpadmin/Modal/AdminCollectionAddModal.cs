// AdminCollectionsAddModule.cs
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
    public class AdminCollectionAddModal : ModalBase
    {
        private static AdminCollectionAddModal instance = null;

        private static AdminCollectionsModule collectionsModule = null;

        private bool submittingForm = false;

        private AdminCollectionAddModal()
            : base("admin", "admin-collection-add-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#admin-collection-add-submit").Click(SubmitForm);
            jQuery.Select("#admin-collection-add-form").Submit(SubmitForm);
            jQuery.Select("#admin-collection-add-browse-btn").Click(BrowseButtonClicked);
        }

        protected override void OnShown()
        {
            jQuery.Select("#admin-collection-add-name").Focus();
        }

        public static void ShowDialog(AdminCollectionsModule collectionsModule)
        {
            if (instance == null)
            {
                instance = new AdminCollectionAddModal();
            }

            instance.InternalShow();
            AdminCollectionAddModal.collectionsModule = collectionsModule;
        }

        public void InternalShow()
        {
            Show();

            jQuery.Select("#admin-collection-add-name").Value("").Focus();
            jQuery.Select("#admin-collection-add-path").Value("");
            jQuery.Select("#admin-collection-add-public").Attribute("checked", "checked");
            jQuery.Select("#admin-collection-add-autoadd").Attribute("checked", "checked");
        }

        private void SubmitForm(jQueryEvent e)
        {
            e.PreventDefault();

            string name = jQuery.Trim(jQuery.Select("#admin-collection-add-name").GetValue());
            string path = jQuery.Select("#admin-collection-add-path").GetValue();

            if (name == null || name == "" || path == null || path == "" || submittingForm)
            {
                return;
            }

            submittingForm = true;

            AdminCollectionAddRequest request = new AdminCollectionAddRequest();
            request.name = name;
            request.path = path;
            request.public_ = jQuery.Select("#admin-collection-add-public").GetAttribute("checked") == "checked";
            request.autoadd = jQuery.Select("#admin-collection-add-autoadd").GetAttribute("checked") == "checked";

            Request.Send(request, SubmitSuccess, SubmitFailure);
        }

        [AlternateSignature]
        private extern void SubmitSuccess(JsonResponse response);
        private void SubmitSuccess(AdminCollectionAddResponse response)
        {
            submittingForm = false;

            switch (response.status)
            {
                case 0:
                    Hide();
                    collectionsModule.Refresh();
                    break;
                case 1:
                    ErrorModal.ShowError(Strings.Get("DuplicateCollectionName"));
                    break;
                case 2:
                    ErrorModal.ShowError(Strings.Get("InvalidCollectionPath"));
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
            AdminFinderModal.ShowDialog(jQuery.Select("#admin-collection-add-path"), -1);
        }
    }
}
