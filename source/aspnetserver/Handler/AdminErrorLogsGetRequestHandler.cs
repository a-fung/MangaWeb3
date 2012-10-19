using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminErrorLogsGetRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminErrorLogsGetRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminErrorLogsGetRequest request = Utility.ParseJson<AdminErrorLogsGetRequest>(jsonString);
            AdminErrorLogsGetResponse response = new AdminErrorLogsGetResponse();
            response.numberOfLogs = Convert.ToInt32(Database.Select("errorlog", null, null, null, "COUNT(*)")[0]["COUNT(*)"]);
            List<ErrorLogJson> logs = new List<ErrorLogJson>();
            foreach (Dictionary<string, object> data in Database.Select("errorlog", null, "`time` DESC", (request.page * request.elementsPerPage) + "," + request.elementsPerPage))
            {
                ErrorLogJson log = new ErrorLogJson();
                log.id = Convert.ToInt32(data["id"]);
                log.time = Convert.ToInt32(data["time"]);
                log.type = Convert.ToString(data["type"]);
                log.source = Convert.ToString(data["source"]);
                log.message = Convert.ToString(data["message"]);
                log.stackTrace = Convert.ToString(data["stacktrace"]);
                logs.Add(log);
            }

            response.logs = logs.ToArray();
            ajax.ReturnJson(response);
        }
    }
}