// AdminCollectionsModule.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Admin.Modal;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Client.Widget;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Module
{
    public class AdminCollectionsModule : AdminModuleBase
    {
        private CollectionJson[] collections;
        private int currentPage;
        private Pagination pagination;

        private static AdminCollectionsModule _instance = null;
        public static AdminCollectionsModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AdminCollectionsModule();
                }

                return _instance;
            }
        }

        private AdminCollectionsModule()
            : base("admin-collections-module")
        {
            jQuery.Select("#admin-collections-add-btn").Click(AddButtonClicked);
            jQuery.Select("#admin-collections-delete-btn").Click(DeleteButtonClicked);
            jQuery.Select("#admin-collections-public-btn").Click(SetPublicButtonClicked);
            jQuery.Select("#admin-collections-private-btn").Click(SetPrivateButtonClicked);
            jQuery.Select("#admin-collections-edit-btn").Click(EditButtonClicked);
            Utility.FixDropdownTouch(jQuery.Select("#admin-collections-action-dropdown"));
            pagination = new Pagination(jQuery.Select("#admin-collections-pagination"), ChangePage, GetTotalPage, "right");
            Refresh();
        }

        protected override void OnShow()
        {
        }

        private void AddButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            AdminCollectionAddModal.ShowDialog(this);
        }

        [AlternateSignature]
        public extern void Refresh(JsonResponse response);
        public void Refresh()
        {
            Request.Send(new AdminCollectionsGetRequest(), GetRequestSuccess);
            jQuery.Select("#admin-collections-tbody").Children().Remove();
            Template.Get("admin", "loading-trow", true).AppendTo(jQuery.Select("#admin-collections-tbody"));
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminCollectionsGetResponse response)
        {
            collections = response.collections;
            ChangePage(1);
            pagination.Refresh(true);
        }

        private void ChangePage(int page)
        {
            currentPage = page;
            jQuery.Select("#admin-collections-tbody").Children().Remove();
            if (collections.Length == 0)
            {
                Template.Get("admin", "noitem-trow", true).AppendTo(jQuery.Select("#admin-collections-tbody"));
            }

            for (int i = (page - 1) * Environment.ElementsPerPage; i < collections.Length && i < page * Environment.ElementsPerPage; i++)
            {
                jQueryObject row = Template.Get("admin", "admin-collections-trow", true).AppendTo(jQuery.Select("#admin-collections-tbody"));
                jQuery.Select(".admin-collections-checkbox", row).Value(collections[i].id.ToString());
                jQuery.Select(".admin-collections-users", row).Attribute("data-id", collections[i].id.ToString()).Click(UsersButtonClicked);
                jQuery.Select(".admin-collections-name", row).Text(collections[i].name);
                jQuery.Select(".admin-collections-path", row).Text(collections[i].path);
                jQuery.Select(".admin-collections-public", row).Text(Strings.Get(collections[i].public_ ? "Yes" : "No")).AddClass(collections[i].public_ ? "label-success" : "");
                jQuery.Select(".admin-collections-autoadd", row).Text(Strings.Get(collections[i].autoadd ? "Yes" : "No")).AddClass(collections[i].autoadd ? "label-success" : "");
            }
        }

        private void UsersButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            string id = jQuery.FromElement(e.Target).GetAttribute("data-id");
            if (!String.IsNullOrEmpty(id))
            {
                AdminCollectionUsersModal.ShowDialog(int.Parse(id, 10));
            }
        }

        private int GetTotalPage()
        {
            if (collections == null || collections.Length == 0)
            {
                return 1;
            }

            return Math.Ceil(collections.Length / Environment.ElementsPerPage);
        }

        private int[] GetSelectedIds()
        {
            return Utility.GetSelectedCheckboxIds("admin-collections-checkbox");
        }

        private void DeleteButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            int[] ids = GetSelectedIds();

            if (ids.Length > 0)
            {
                ConfirmModal.Show(Strings.Get("DeleteCollectionsConfirm"), DeleteConfirm);
            }
        }

        private void DeleteConfirm(bool confirm)
        {
            int[] ids = GetSelectedIds();

            if (confirm && ids.Length > 0)
            {
                AdminCollectionsDeleteRequest request = new AdminCollectionsDeleteRequest();
                request.ids = ids;
                Request.Send(request, Refresh);
            }
        }

        private void SetPublicButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            SetPublicOrPrivateClicked(true);
        }

        private void SetPrivateButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            SetPublicOrPrivateClicked(false);
        }

        private void SetPublicOrPrivateClicked(bool public_)
        {
            int[] ids = GetSelectedIds();

            if (ids.Length > 0)
            {
                AdminCollectionsSetPublicRequest request = new AdminCollectionsSetPublicRequest();
                request.ids = ids;
                request.public_ = public_;
                Request.Send(request, Refresh);
            }
        }

        private void EditButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            int[] ids = GetSelectedIds();

            if (ids.Length > 1)
            {
                ErrorModal.ShowError(Strings.Get("SelectSingleItem"));
            }
            else if (ids.Length == 1)
            {
                AdminCollectionEditNameModal.ShowDialog(this, ids[0]);
            }
        }
    }
}
