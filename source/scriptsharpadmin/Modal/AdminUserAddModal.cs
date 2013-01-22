// AdminUserAddModal.cs
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
    public class AdminUserAddModal : ModalBase
    {
        private static AdminUserAddModal instance = null;

        private static AdminUsersModule usersModule = null;

        private bool submittingForm = false;

        private AdminUserAddModal()
            : base("admin", "admin-user-add-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#admin-user-add-submit").Click(SubmitForm);
            jQuery.Select("#admin-user-add-form").Submit(SubmitForm);
        }

        protected override void OnShown()
        {
            jQuery.Select("#admin-user-add-name").Focus();
        }

        public static void ShowDialog(AdminUsersModule usersModule)
        {
            if (instance == null)
            {
                instance = new AdminUserAddModal();
            }

            instance.InternalShow();
            AdminUserAddModal.usersModule = usersModule;
        }

        public void InternalShow()
        {
            Show();

            jQuery.Select("#admin-user-add-name").Value("").Focus();
            jQuery.Select("#admin-user-add-password").Value("");
            jQuery.Select("#admin-user-add-password2").Value("");
            jQuery.Select("#admin-user-add-administrator").RemoveAttr("checked");
        }

        private void SubmitForm(jQueryEvent e)
        {
            e.PreventDefault();

            string name = jQuery.Trim(jQuery.Select("#admin-user-add-name").GetValue());
            string password = jQuery.Select("#admin-user-add-password").GetValue();
            string password2 = jQuery.Select("#admin-user-add-password2").GetValue();

            if (name == "" || password == "" || password2 == "" || submittingForm)
            {
                return;
            }

            RegularExpression regex = new RegularExpression("[^a-zA-Z0-9]");

            if (regex.Test(name) || password.Length < 8 || password != password2)
            {
                ErrorModal.ShowError(Strings.Get("AddUserInvalidInput"));
                return;
            }

            submittingForm = true;

            AdminUserAddRequest request = new AdminUserAddRequest();
            request.username = name;
            request.password = password;
            request.password2 = password2;
            request.admin = jQuery.Select("#admin-user-add-administrator").Is(":checked");

            Request.Send(request, SubmitSuccess, SubmitFailure);
        }

        [AlternateSignature]
        private extern void SubmitSuccess(JsonResponse response);
        private void SubmitSuccess(AdminUserAddResponse response)
        {
            submittingForm = false;

            switch (response.status)
            {
                case 0:
                    Hide();
                    usersModule.Refresh();
                    break;
                case 1:
                    ErrorModal.ShowError(Strings.Get("DuplicateUserName"));
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
