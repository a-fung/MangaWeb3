// AdminCollectionEditNameModal.cs
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
    public class AdminCollectionEditNameModal : ModalBase
    {
        private static AdminCollectionEditNameModal instance = null;

        private static AdminCollectionsModule collectionsModule = null;

        private bool submittingForm = false;
        private int collectionId;

        private AdminCollectionEditNameModal()
            : base("admin", "admin-collection-editname-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#admin-collection-editname-submit").Click(SubmitForm);
            jQuery.Select("#admin-collection-editname-form").Submit(SubmitForm);
        }

        protected override void OnShown()
        {
            jQuery.Select("#admin-collection-editname-name").Focus();
        }

        public static void ShowDialog(AdminCollectionsModule collectionsModule, int id)
        {
            if (instance == null)
            {
                instance = new AdminCollectionEditNameModal();
            }

            instance.InternalShow(id);
            AdminCollectionEditNameModal.collectionsModule = collectionsModule;
        }

        public void InternalShow(int id)
        {
            AdminCollectionEditNameRequest request = new AdminCollectionEditNameRequest();
            collectionId = request.id = id;

            Request.Send(request, GetNameSuccess);
        }

        [AlternateSignature]
        private extern void GetNameSuccess(JsonResponse response);
        private void GetNameSuccess(AdminCollectionEditNameResponse response)
        {
            Show();
            jQuery.Select("#admin-collection-editname-name").Value(response.name).Focus();
        }

        private void SubmitForm(jQueryEvent e)
        {
            e.PreventDefault();
            string name = jQuery.Select("#admin-collection-editname-name").GetValue();
            if (name != "" && !submittingForm)
            {
                AdminCollectionEditNameRequest request = new AdminCollectionEditNameRequest();
                request.id = collectionId;
                request.name = name;
                submittingForm = true;

                Request.Send(request, SubmitSuccess, SubmitFailure);
            }
        }

        [AlternateSignature]
        private extern void SubmitSuccess(JsonResponse response);
        private void SubmitSuccess(AdminCollectionEditNameResponse response)
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
    }
}
