package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.LoginRequest;
import afung.mangaWeb3.common.LoginResponse;
import afung.mangaWeb3.server.SessionWrapper;
import afung.mangaWeb3.server.User;

/**
 * ...
 * @author a-fung
 */

class LoginRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return LoginRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        var request:LoginRequest = Utility.ParseJson(jsonString);
        
        var user:User = null;
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
        
        var response:LoginResponse = new LoginResponse();
        
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