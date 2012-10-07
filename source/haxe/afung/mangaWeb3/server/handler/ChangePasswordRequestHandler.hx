package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.ChangePasswordRequest;
import afung.mangaWeb3.common.ChangePasswordResponse;
import afung.mangaWeb3.server.User;

/**
 * ...
 * @author a-fung
 */

class ChangePasswordRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return ChangePasswordRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var user:User = User.GetCurrentUser(ajax);
        if (user == null)
        {
            ajax.BadRequest();
            return;
        }

        var request:ChangePasswordRequest = Utility.ParseJson(jsonString);
        if (request.currentPassword == null || request.currentPassword == "" || request.newPassword == null || request.newPassword.length < 8 || request.newPassword != request.newPassword2)
        {
            ajax.BadRequest();
            return;
        }

        var response:ChangePasswordResponse = new ChangePasswordResponse();

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