using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;
using afung.MangaWeb3.Server.Handler;
using Newtonsoft.Json;

namespace afung.MangaWeb3.Server
{
    public class AjaxBase : System.Web.UI.Page
    {
        protected void HandleRequest(HandlerBase[] handlers)
        {
            string jsonString = RequestParams("j");
            if (String.IsNullOrEmpty(jsonString) || String.IsNullOrEmpty(jsonString.Trim()))
            {
                BadRequest(); return;
            }

            JsonRequest jsonRequest = Utility.ParseJson<JsonRequest>(jsonString);

            foreach (HandlerBase handler in handlers)
            {
                if (handler.CanHandle(jsonRequest))
                {
                    handler.HandleRequest(jsonString, this);
                    return;
                }
            }

            BadRequest();
        }

        private string RequestParams(string name)
        {
            return Request.Params[name];
        }

        public void BadRequest()
        {
            Response.StatusCode = 400;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write("Bad Request");
            Response.End();
        }

        public void Redirect(string url)
        {
            Response.Redirect(url, true);
        }

        public void ReturnJson(JsonResponse response)
        {
            string output = JsonConvert.SerializeObject(response);

            Response.StatusCode = 200;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/json";
            Response.Write(output);
            Response.End();
        }
    }
}