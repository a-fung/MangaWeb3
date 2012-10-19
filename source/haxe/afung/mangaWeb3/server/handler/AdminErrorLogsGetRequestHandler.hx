package afung.mangaWeb3.server.handler;

import afung.mangaWeb3.common.AdminErrorLogsGetRequest;
import afung.mangaWeb3.common.AdminErrorLogsGetResponse;
import afung.mangaWeb3.common.ErrorLogJson;

/**
 * ...
 * @author a-fung
 */

class AdminErrorLogsGetRequestHandler extends HandlerBase
{
    public override function GetHandleRequestType():Class<Dynamic>
    {
        return AdminErrorLogsGetRequest;
    }
    
    public override function HandleRequest(jsonString:String, ajax:AjaxBase):Void 
    {
        if (!User.IsAdminLoggedIn(ajax))
        {
            ajax.Unauthorized();
            return;
        }
        
        var request:AdminErrorLogsGetRequest = Utility.ParseJson(jsonString);
        var response:AdminErrorLogsGetResponse = new AdminErrorLogsGetResponse();
        response.numberOfLogs = Std.parseInt(Database.Select("errorlog", null, null, null, "COUNT(*)")[0].get("COUNT(*)"));
        var logs:Array<ErrorLogJson> = [];
        for (data in Database.Select("errorlog", null, "`time` DESC", (request.page * request.elementsPerPage) + "," + request.elementsPerPage))
        {
            var log:ErrorLogJson = new ErrorLogJson();
            log.id = Std.parseInt(data.get("id"));
            log.time = Std.parseInt(data.get("time"));
            log.type = Std.string(data.get("type"));
            log.source = Std.string(data.get("source"));
            log.message = Std.string(data.get("message"));
            log.stackTrace = Std.string(data.get("stacktrace"));
            logs.push(log);
        }
        
        response.logs = logs;
        ajax.ReturnJson(response);
    }
}