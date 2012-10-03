// AdminUserCollectionsModal.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Admin.Module;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Client.Widget;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Modal
{
    public class AdminUserCollectionsModal : ModalBase
    {
        private static AdminUserCollectionsModal instance = null;

        private int uid;

        private string username;

        private string[] collectionNames;

        private Pagination pagination;

        private CollectionUserJson[] data;

        private int currentPage;

        private bool submittingForm;

        private AdminUserCollectionsModal()
            : base("admin", "admin-user-collections-modal")
        {
        }

        protected override void Initialize()
        {
            pagination = new Pagination(jQuery.Select("#admin-user-collections-pagination"), ChangePage, GetTotalPage, "right");
            jQuery.Select("#admin-user-collections-form").Submit(SubmitAddForm);
            jQuery.Select("#admin-user-collections-add-submit").Click(SubmitAddForm);
            jQuery.Select("#admin-user-collections-delete-btn").Click(DeleteButtonClicked);
            jQuery.Select("#admin-user-collections-allow-btn").Click(AllowButtonClicked);
            jQuery.Select("#admin-user-collections-deny-btn").Click(DenyButtonClicked);
            Utility.FixDropdownTouch(jQuery.Select("#admin-user-collections-action-dropdown"));
        }

        private int GetTotalPage()
        {
            if (data == null || data.Length == 0)
            {
                return 1;
            }

            return Math.Ceil(data.Length / Environment.ModalElementsPerPage);
        }

        public static void ShowDialog(int uid)
        {
            if (instance == null)
            {
                instance = new AdminUserCollectionsModal();
            }

            instance.InternalShow(uid);
        }

        public void InternalShow(int uid)
        {
            this.uid = uid;
            Refresh();
        }

        [AlternateSignature]
        public extern void Refresh(JsonResponse response);
        public void Refresh()
        {
            AdminCollectionsUsersGetRequest request = new AdminCollectionsUsersGetRequest();
            request.t = 1;
            request.id = uid;

            Request.Send(request, GetRequestSuccess);
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminCollectionsUsersGetResponse response)
        {
            Show();
            jQuery.Select("#admin-user-collections-name").Text(username = response.name);
            ((BootstrapTypeahead)((jQueryBootstrap)jQuery.Select("#admin-user-collections-add-collection").Value("")).Typeahead().GetDataValue("typeahead")).Source = collectionNames = response.names;

            data = response.data;
            ChangePage(1);
            pagination.Refresh(true);
        }

        private void ChangePage(int page)
        {
            currentPage = page;
            jQuery.Select("#admin-user-collections-tbody").Children().Remove();
            for (int i = (page - 1) * Environment.ModalElementsPerPage; i < data.Length && i < page * Environment.ModalElementsPerPage; i++)
            {
                jQueryObject row = Template.Get("admin", "admin-user-collections-trow", true).AppendTo(jQuery.Select("#admin-user-collections-tbody"));
                jQuery.Select(".admin-user-collections-checkbox", row).Value(data[i].cid.ToString());
                jQuery.Select(".admin-user-collections-collection", row).Text(data[i].collectionName);
                jQuery.Select(".admin-user-collections-access", row).Text(Strings.Get(data[i].access ? "Yes" : "No")).AddClass(data[i].access ? "label-success" : "");
            }
            Show();
        }

        private void SubmitAddForm(jQueryEvent e)
        {
            e.PreventDefault();

            string collectionName = jQuery.Select("#admin-user-collections-add-collection").GetValue();
            if (collectionNames.Contains(collectionName) && !submittingForm)
            {
                submittingForm = true;

                AdminCollectionUserAddRequest request = new AdminCollectionUserAddRequest();
                request.username = username;
                request.collectionName = collectionName;
                request.access = jQuery.Select("#admin-user-collections-add-access").GetValue() == "true";

                Request.Send(request, SubmitAddFormSuccess, SubmitAddFormFailure);
            }
        }

        private void SubmitAddFormSuccess(JsonResponse response)
        {
            submittingForm = false;
            Refresh();
        }

        private void SubmitAddFormFailure(Exception error)
        {
            submittingForm = false;
            ErrorModal.ShowError(error.ToString());
        }

        private void AllowButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            InternalAccessButtonClicked(true);
        }

        private void DenyButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            InternalAccessButtonClicked(false);
        }

        private void InternalAccessButtonClicked(bool access)
        {
            int[] ids = GetSelectedIds();
            if (ids.Length > 0)
            {
                AdminCollectionsUsersAccessRequest request = new AdminCollectionsUsersAccessRequest();
                request.t = 1;
                request.id = uid;
                request.ids = ids;
                request.access = access;
                Request.Send(request, Refresh);
            }
        }

        private void DeleteButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            int[] ids = GetSelectedIds();
            if (ids.Length > 0)
            {
                ConfirmModal.Show(Strings.Get("DeleteItemsConfirm"), DeleteConfirm);
            }
        }

        private void DeleteConfirm(bool confirm)
        {
            int[] ids = GetSelectedIds();

            if (confirm && ids.Length > 0)
            {
                AdminCollectionsUsersDeleteRequest request = new AdminCollectionsUsersDeleteRequest();
                request.t = 1;
                request.id = uid;
                request.ids = ids;
                Request.Send(request, Refresh);
            }
        }

        private int[] GetSelectedIds()
        {
            return Utility.GetSelectedCheckboxIds("admin-user-collections-checkbox");
        }
    }
}
