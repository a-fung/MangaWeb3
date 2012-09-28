// AdminCollectionUsersModal.cs
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
    public class AdminCollectionUsersModal : ModalBase
    {
        private static AdminCollectionUsersModal instance = null;

        private int cid;

        private string collectionName;

        private string[] usernames;

        private Pagination pagination;

        private CollectionUserJson[] data;

        private int currentPage;

        private bool submittingForm;

        private AdminCollectionUsersModal()
            : base("admin", "admin-collection-users-modal")
        {
        }

        protected override void Initialize()
        {
            pagination = new Pagination(jQuery.Select("#admin-collection-users-pagination"), ChangePage, GetTotalPage, "right");
            jQuery.Select("#admin-collection-users-add-submit").Click(SubmitAddForm);
            jQuery.Select("#admin-collection-users-form").Submit(SubmitAddForm);
            jQuery.Select("#admin-collection-users-delete-btn").Click(DeleteButtonClicked);
            jQuery.Select("#admin-collection-users-allow-btn").Click(AllowButtonClicked);
            jQuery.Select("#admin-collection-users-deny-btn").Click(DenyButtonClicked);
            Utility.FixDropdownTouch(jQuery.Select("#admin-collection-users-action-dropdown"));
        }

        private int GetTotalPage()
        {
            if (data == null || data.Length == 0)
            {
                return 1;
            }

            return Math.Ceil(data.Length / Environment.ModalElementsPerPage);
        }

        public static void ShowDialog(int cid)
        {
            if (instance == null)
            {
                instance = new AdminCollectionUsersModal();
            }

            instance.InternalShow(cid);
        }

        public void InternalShow(int cid)
        {
            Show();
            this.cid = cid;
            Refresh();
        }

        [AlternateSignature]
        public extern void Refresh(JsonResponse response);
        public void Refresh()
        {
            AdminCollectionsUsersGetRequest request = new AdminCollectionsUsersGetRequest();
            request.t = 0;
            request.id = cid;

            Request.Send(request, GetRequestSuccess);
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminCollectionsUsersGetResponse response)
        {
            jQuery.Select("#admin-collection-users-name").Text(collectionName = response.name);
            ((BootstrapTypeahead)((jQueryBootstrap)jQuery.Select("#admin-collection-users-add-user").Value("")).Typeahead().GetDataValue("typeahead")).Source = usernames = response.names;

            data = response.data;
            ChangePage(1);
            pagination.Refresh(true);
        }

        private void ChangePage(int page)
        {
            currentPage = page;
            jQuery.Select("#admin-collection-users-tbody").Children().Remove();
            for (int i = (page - 1) * Environment.ModalElementsPerPage; i < data.Length && i < page * Environment.ModalElementsPerPage; i++)
            {
                jQueryObject row = Template.Get("admin", "admin-collection-users-trow", true).AppendTo(jQuery.Select("#admin-collection-users-tbody"));
                jQuery.Select(".admin-collection-users-checkbox", row).Value(data[i].uid.ToString());
                jQuery.Select(".admin-collection-users-username", row).Text(data[i].username);
                jQuery.Select(".admin-collection-users-access", row).Text(Strings.Get(data[i].access ? "Yes" : "No")).AddClass(data[i].access ? "label-success" : "");
            }
            Show();
        }

        private void SubmitAddForm(jQueryEvent e)
        {
            e.PreventDefault();

            string username = jQuery.Select("#admin-collection-users-add-user").GetValue();
            if (usernames.Contains(username) && !submittingForm)
            {
                submittingForm = true;

                AdminCollectionUserAddRequest request = new AdminCollectionUserAddRequest();
                request.username = username;
                request.collectionName = collectionName;
                request.access = jQuery.Select("#admin-collection-users-add-access").GetValue() == "true";

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
                request.t = 0;
                request.id = cid;
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
                request.t = 0;
                request.id = cid;
                request.ids = ids;
                Request.Send(request, Refresh);
            }
        }

        private int[] GetSelectedIds()
        {
            return Utility.GetSelectedCheckboxIds("admin-collection-users-checkbox");
        }
    }
}
