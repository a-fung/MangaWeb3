using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminUserAddRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminUserAddRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminUserAddRequest request = Utility.ParseJson<AdminUserAddRequest>(jsonString);
            Regex regex = new Regex("[^a-zA-Z0-9]");
            if (request.username == null || request.username == "" || regex.IsMatch(request.username) || request.username.Length > 30 || request.password == null || request.password.Length < 8 || request.password != request.password2)
            {
                ajax.BadRequest();
                return;
            }

            AdminUserAddResponse response = new AdminUserAddResponse();

            if (User.GetUser(request.username) != null)
            {
                response.status = 1;
            }
            else
            {
                response.status = 0;
                User.CreateNewUser(request.username, request.password, request.admin).Save();
            }

            ajax.ReturnJson(response);
        }
    }
}