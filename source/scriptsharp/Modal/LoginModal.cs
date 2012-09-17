// LoginModal.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Modal
{
    public class LoginModal : ModalBase
    {
        private static LoginModal instance = null;

        private Action<LoginResponse> successCallback;
        private Action<Exception> failureCallback;

        private LoginModal()
            : base("client", "login-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#login-modal-submit").Click(SubmitButtonClicked);
            jQuery.Select("#login-modal-cancel").Click(CancelButtonClicked);
        }

        public static void Prompt(Action<LoginResponse> successCallback, Action<Exception> failureCallback)
        {
            if (instance == null)
            {
                instance = new LoginModal();
            }
            instance.InternalPrompt(successCallback, failureCallback);
        }

        public void InternalPrompt(Action<LoginResponse> successCallback, Action<Exception> failureCallback)
        {
            this.successCallback = successCallback;
            this.failureCallback = failureCallback;

            ShowStatic();
        }

        [AlternateSignature]
        private extern void SubmitButtonClicked(jQueryEvent e);
        private void SubmitButtonClicked()
        {
        }

        [AlternateSignature]
        private extern void CancelButtonClicked(jQueryEvent e);
        private void CancelButtonClicked()
        {
            Hide();
            if (!Script.IsNullOrUndefined(failureCallback))
            {
                failureCallback(new Exception(Strings.Get("LoginCancelled")));
            }
        }
    }
}
