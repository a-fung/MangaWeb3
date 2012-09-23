using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class LoginRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(LoginRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            LoginRequest request = Utility.ParseJson<LoginRequest>(jsonString);

            User user = null;
            if (request.username == null)
            {
                if (request.password == null)
                {
                    user = User.GetUser(SessionWrapper.GetUserName(ajax));
                }
            }
            else
            {
                user = User.GetUser(request.username, request.password);
            }

            LoginResponse response = new LoginResponse();

            if (user == null)
            {
                response.username = "";
                response.admin = false;
            }
            else
            {
                response.username = user.Username;
                response.admin = user.Admin;
            }

            SessionWrapper.SetUserName(ajax, response.username);

            ajax.ReturnJson(response);
        }
    }
}