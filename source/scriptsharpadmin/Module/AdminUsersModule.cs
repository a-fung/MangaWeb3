// AdminUsersModule.cs
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
    public class AdminUsersModule : AdminModuleBase
    {
        private UserJson[] users;

        private int currentPage = 1;
        private Pagination pagination;
        private string currentUserName;

        private static AdminUsersModule _instance = null;
        public static AdminUsersModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AdminUsersModule();
                }

                return _instance;
            }
        }

        private AdminUsersModule()
            : base("admin-users-module")
        {
            LoginModal.GetUserName(delegate(LoginResponse response)
            {
                currentUserName = response.username;
            });
            jQuery.Select("#admin-users-add-btn").Click(AddButtonClicked);
            jQuery.Select("#admin-users-delete-btn").Click(DeleteButtonClicked);
            jQuery.Select("#admin-users-setasadmin-btn").Click(SetAsAdminButtonClicked);
            jQuery.Select("#admin-users-setasuser-btn").Click(SetAsUserButtonClicked);
            Utility.FixDropdownTouch(jQuery.Select("#admin-users-action-dropdown"));
            pagination = new Pagination(jQuery.Select("#admin-users-pagination"), ChangePage, GetTotalPage, "right");
            Refresh();
        }

        protected override void OnShow()
        {
        }

        private void ChangePage(int page)
        {
            currentPage = page;
            jQuery.Select("#admin-users-tbody").Children().Remove();
            if (users.Length == 0)
            {
                Template.Get("admin", "noitem-trow", true).AppendTo(jQuery.Select("#admin-users-tbody"));
            }

            for (int i = (page - 1) * Environment.ElementsPerPage; i < users.Length && i < page * Environment.ElementsPerPage; i++)
            {
                jQueryObject row = Template.Get("admin", "admin-users-trow", true).AppendTo(jQuery.Select("#admin-users-tbody"));
                jQuery.Select(".admin-users-checkbox", row).Value(users[i].id.ToString());
                if (currentUserName == users[i].username)
                {
                    jQuery.Select(".admin-users-checkbox", row).Attribute("disabled", "disabled");
                }

                jQuery.Select(".admin-users-collections", row).Attribute("data-id", users[i].id.ToString()).Click(CollectionsButtonClicked);
                jQuery.Select(".admin-users-username", row).Text(users[i].username);
                jQuery.Select(".admin-users-administrator", row).Text(Strings.Get(users[i].admin ? "Yes" : "No")).AddClass(users[i].admin ? "label-info" : "");
            }
        }

        private int GetTotalPage()
        {
            if (users == null || users.Length == 0)
            {
                return 1;
            }

            return Math.Ceil(users.Length / Environment.ElementsPerPage);
        }

        [AlternateSignature]
        public extern void Refresh(JsonResponse response);
        public void Refresh()
        {
            Request.Send(new AdminUsersGetRequest(), GetRequestSuccess);
            jQuery.Select("#admin-users-tbody").Children().Remove();
            Template.Get("admin", "loading-trow", true).AppendTo(jQuery.Select("#admin-users-tbody"));
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminUsersGetResponse response)
        {
            users = response.users;
            ChangePage(currentPage);
            pagination.Refresh();
        }

        private void CollectionsButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            string id = jQuery.FromElement(e.Target).GetAttribute("data-id");
            if (!String.IsNullOrEmpty(id))
            {
                AdminUserCollectionsModal.ShowDialog(int.Parse(id, 10));
            }
        }

        private void AddButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            AdminUserAddModal.ShowDialog(this);
        }

        private void DeleteButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            int[] ids = GetSelectedIds();

            if (ids.Length > 0)
            {
                ConfirmModal.Show(Strings.Get("DeleteUsersConfirm"), DeleteConfirm);
            }
        }

        private void DeleteConfirm(bool confirm)
        {
            int[] ids = GetSelectedIds();

            if (confirm && ids.Length > 0)
            {
                AdminUsersDeleteRequest request = new AdminUsersDeleteRequest();
                request.ids = ids;
                Request.Send(request, Refresh);
            }
        }

        private int[] GetSelectedIds()
        {
            return Utility.GetSelectedCheckboxIds("admin-users-checkbox");
        }

        private void SetAsAdminButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            InternalSetAdmin(true);
        }

        private void SetAsUserButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            InternalSetAdmin(false);
        }

        private void InternalSetAdmin(bool admin)
        {
            int[] ids = GetSelectedIds();

            if (ids.Length > 0)
            {
                AdminUsersSetAdminRequest request = new AdminUsersSetAdminRequest();
                request.ids = ids;
                request.admin = admin;
                Request.Send(request, Refresh);
            }
        }
    }
}
