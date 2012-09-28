package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminFinderRequest;
import afung.mangaWeb3.common.AdminFinderResponse;
import afung.mangaWeb3.server.Collection;
import afung.mangaWeb3.server.SessionWrapper;
import php.Lib;
import php.NativeArray;

/**
 * ...
 * @author a-fung
 */

class AdminFinderRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminFinderRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminFinderRequest = Utility.ParseJson(jsonString);
        var response:AdminFinderResponse = new AdminFinderResponse();
        var collection:Collection = null;
        var finderData:NativeArray;
        
        if (request.cid != -1)
        {
            collection = Collection.GetById(request.cid);
            if (collection == null)
            {
                ajax.BadRequest();
                return;
            }
            
            finderData = Lib.toPhpArray([Lib.toPhpArray([collection.Name, collection.Path])]);
        }
        else
        {
            finderData = Lib.toPhpArray([Lib.toPhpArray(["/", "/"])]);
        }
        
        var tokenBuilder:StringBuf = new StringBuf();
        for (i in 0...32)
        {
            var n:Int = Std.random(36);
            if (n < 10)
            {
                tokenBuilder.add(Std.string(n));
            }
            else
            {
                tokenBuilder.add(String.fromCharCode(55 + n));
            }
        }
        
        response.token = tokenBuilder.toString();
        
        SessionWrapper.SetFinderData(ajax, response.token, finderData);
        
        ajax.ReturnJson(response);
    }
}