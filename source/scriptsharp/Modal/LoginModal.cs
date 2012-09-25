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
        private static LoginResponse userInfo = null;

        private static Action<LoginResponse> successCallback;
        private static Action<Exception> failureCallback;
        private static bool showPrompt;

        private bool loggingIn = false;

        private LoginModal()
            : base("client", "login-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#login-modal-submit").Click(SubmitLogin);
            jQuery.Select("#login-modal-form").Submit(SubmitLogin);
            jQuery.Select("#login-modal-cancel").Click(CancelButtonClicked);
        }

        [AlternateSignature]
        public extern static void GetUserName(Action<LoginResponse> successCallback);
        [AlternateSignature]
        public extern static void GetUserName(Action<LoginResponse> successCallback, Action<Exception> failureCallback);
        [AlternateSignature]
        public extern static void GetUserName(Action<LoginResponse> successCallback, Action<Exception> failureCallback, bool showPrompt);
        public static void GetUserName(Action<LoginResponse> successCallback, Action<Exception> failureCallback, bool showPrompt, bool skipCurrentUserInfo)
        {
            LoginModal.successCallback = successCallback;
            LoginModal.failureCallback = failureCallback;
            LoginModal.showPrompt = showPrompt;

            if (skipCurrentUserInfo || userInfo != null)
            {
                CheckUserNameSuccess(userInfo, skipCurrentUserInfo);
            }
            else
            {
                LoginRequest request = new LoginRequest();
                Request.Send(request, CheckUserNameSuccess, CheckUserNameFailure);
            }
        }

        [AlternateSignature]
        private extern static void CheckUserNameSuccess(JsonResponse response);
        private static void CheckUserNameSuccess(LoginResponse response, bool skipCurrentUserInfo)
        {
            userInfo = response;

            if (skipCurrentUserInfo || (String.IsNullOrEmpty(response.username) && showPrompt))
            {
                Prompt();
            }
            else
            {
                successCallback(response);
            }
        }

        private static void CheckUserNameFailure(Exception error)
        {
            if (!Script.IsNullOrUndefined(failureCallback))
            {
                failureCallback(error);
            }
        }

        private static void Prompt()
        {
            if (instance == null)
            {
                instance = new LoginModal();
            }
            instance.InternalPrompt();
        }

        public void InternalPrompt()
        {
            ShowStatic();
        }

        private void SubmitLogin(jQueryEvent e)
        {
            e.PreventDefault();

            if (loggingIn)
            {
                return;
            }

            LoginRequest request = new LoginRequest();
            request.username = jQuery.Select("#login-modal-username").GetValue();
            request.password = jQuery.Select("#login-modal-password").GetValue();

            if (String.IsNullOrEmpty(request.username) || String.IsNullOrEmpty(request.password))
            {
                return;
            }

            loggingIn = true;
            Request.Send(request, LoginSuccess, LoginFailure);
        }

        [AlternateSignature]
        private extern void CancelButtonClicked(jQueryEvent e);
        private void CancelButtonClicked()
        {
            if (loggingIn)
            {
                return;
            }

            Hide();
            if (!Script.IsNullOrUndefined(failureCallback))
            {
                failureCallback(new Exception(Strings.Get("LoginCancelled")));
            }
        }

        [AlternateSignature]
        private extern void LoginSuccess(JsonResponse response);
        private void LoginSuccess(LoginResponse response)
        {
            loggingIn = false;
            userInfo = response;

            if (String.IsNullOrEmpty(response.username))
            {
                ErrorModal.ShowError(Strings.Get("WrongUserNameOrPassword"));
            }
            else
            {
                Hide();
                successCallback(response);
            }
        }

        private void LoginFailure(Exception error)
        {
            loggingIn = false;
            ErrorModal.ShowError(String.Format(Strings.Get("LoginFailed"), error));
        }

        private void InternalHide()
        {
            jQuery.Select("#login-modal-username").Value("");
            jQuery.Select("#login-modal-password").Value("");
            Hide();
        }
    }
}
