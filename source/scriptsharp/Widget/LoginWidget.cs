// LoginWidget.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Widget
{
    public class LoginWidget
    {
        private jQueryObject loginButton;
        private jQueryObject logoutDropdown;

        private static List<LoginWidget> instances;

        public LoginWidget(jQueryObject parent)
        {
            if (instances == null)
            {
                instances = new List<LoginWidget>();
            }

            loginButton = Template.Get("client", "nav-login-button", true).AppendTo(parent).Hide();
            logoutDropdown = Template.Get("client", "nav-logout-dropdown", true).AppendTo(parent).Hide();

            jQuery.Select(".nav-login", loginButton).Click(LoginButtonClicked);
            jQuery.Select(".nav-logout", logoutDropdown).Click(LogoutButtonClicked);
            jQuery.Select(".nav-change-password", logoutDropdown).Click(ChangePasswordButtonClicked);

            Refresh();
            instances.Add(this);
        }

        private void LoginButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            LoginModal.GetUserName(
                delegate(LoginResponse userinfo)
                {
                },
                null,
                true);
        }

        private void LogoutButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            LoginRequest request = new LoginRequest();
            request.password = "logout";

            Request.Send(request, LogoutSuccessful, LogoutSuccessful);
        }

        [AlternateSignature]
        private extern void LogoutSuccessful(Exception error);
        [AlternateSignature]
        private extern void LogoutSuccessful(JsonResponse response);
        private void LogoutSuccessful()
        {
            // redirect to index.html
            Window.Location.Href = "index.html";
        }

        private void ChangePasswordButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            ChangePasswordModal.ShowDialog();
        }

        public void Refresh()
        {
            LoginModal.GetUserName(delegate(LoginResponse userinfo)
            {
                if (String.IsNullOrEmpty(userinfo.username))
                {
                    loginButton.Show();
                    logoutDropdown.Hide();
                }
                else
                {
                    loginButton.Hide();
                    logoutDropdown.Show();
                    jQuery.Select(".nav-user-username", logoutDropdown).Text(userinfo.username);
                }
            });
        }

        public static void RefreshAll()
        {
            if (instances != null)
            {
                foreach (LoginWidget instance in instances)
                {
                    instance.Refresh();
                }
            }
        }
    }
}
