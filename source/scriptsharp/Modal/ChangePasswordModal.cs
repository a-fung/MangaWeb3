// ChangePasswordModal.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Modal
{
    public class ChangePasswordModal : ModalBase
    {
        private static ChangePasswordModal instance = null;

        private bool submittingForm = false;

        private ChangePasswordModal()
            : base("client", "change-password-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#change-password-submit").Click(SubmitForm);
            jQuery.Select("#change-password-form").Submit(SubmitForm);
        }

        public static void ShowDialog()
        {
            if (instance == null)
            {
                instance = new ChangePasswordModal();
            }

            instance.InternalShow();
        }

        public void InternalShow()
        {
            jQuery.Select("#change-password-current-password").Value("");
            jQuery.Select("#change-password-new-password").Value("");
            jQuery.Select("#change-password-new-password2").Value("");
            Show();
        }

        private void SubmitForm(jQueryEvent e)
        {
            e.PreventDefault();

            string currentPassword = jQuery.Select("#change-password-current-password").GetValue();
            string password = jQuery.Select("#change-password-new-password").GetValue();
            string password2 = jQuery.Select("#change-password-new-password2").GetValue();

            if (currentPassword == "" || password == "" || password2 == "" || submittingForm)
            {
                return;
            }

            if (password.Length < 8 || password != password2)
            {
                ErrorModal.ShowError(Strings.Get("AddUserInvalidInput"));
                return;
            }

            submittingForm = true;

            ChangePasswordRequest request = new ChangePasswordRequest();
            request.currentPassword = currentPassword;
            request.newPassword = password;
            request.newPassword2 = password2;

            Request.Send(request, SubmitSuccess, SubmitFailure);
        }

        [AlternateSignature]
        private extern void SubmitSuccess(JsonResponse response);
        private void SubmitSuccess(ChangePasswordResponse response)
        {
            submittingForm = false;

            switch (response.status)
            {
                case 0:
                    Hide();
                    break;
                case 1:
                    ErrorModal.ShowError(Strings.Get("WrongPassword"));
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
