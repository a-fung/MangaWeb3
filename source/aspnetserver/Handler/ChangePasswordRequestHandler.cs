using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class ChangePasswordRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(ChangePasswordRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            User user = User.GetCurrentUser(ajax);
            if (user == null)
            {
                ajax.BadRequest();
                return;
            }

            ChangePasswordRequest request = Utility.ParseJson<ChangePasswordRequest>(jsonString);
            if (request.currentPassword == null || request.currentPassword == "" || request.newPassword == null || request.newPassword.Length < 8 || request.newPassword != request.newPassword2)
            {
                ajax.BadRequest();
                return;
            }

            ChangePasswordResponse response = new ChangePasswordResponse();

            if (!user.MatchPassword(request.currentPassword))
            {
                response.status = 1;
            }
            else
            {
                response.status = 0;
                user.SetPassword(request.newPassword);
                user.Save();
            }

            ajax.ReturnJson(response);
        }
    }
}