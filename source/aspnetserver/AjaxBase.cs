
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;
using afung.MangaWeb3.Server.Handler;
using Newtonsoft.Json;

namespace afung.MangaWeb3.Server
{
    /// <summary>
    /// The base class for Http request server entry point
    /// </summary>
    public abstract class AjaxBase : System.Web.UI.Page
    {
        /// <summary>
        /// The Directory Path of the current page on the server. It's set when the server app handles the first request
        /// </summary>
        public static string DirectoryPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Called when the page loads
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(DirectoryPath))
            {
                DirectoryPath = Server.MapPath(".");
            }

            PageLoad();
        }

        /// <summary>
        /// Handle the json request using a list of handlers
        /// </summary>
        /// <param name="handlers">The array of handlers</param>
        protected void HandleRequest(HandlerBase[] handlers)
        {
            try
            {
                string jsonString = RequestParams("j");
                if (String.IsNullOrEmpty(jsonString) || String.IsNullOrEmpty(jsonString.Trim()))
                {
                    BadRequest();
                    return;
                }

                JsonRequest jsonRequest = Utility.ParseJson<JsonRequest>(jsonString);
                if (jsonRequest == null || String.IsNullOrEmpty(jsonRequest.type))
                {
                    BadRequest();
                    return;
                }

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
            catch (Exception ex)
            {
                Utility.TryLogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Get the value of a request parameter
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <returns>The value</returns>
        private string RequestParams(string name)
        {
            return Request.Params[name];
        }

        /// <summary>
        /// Return "bad request" to the client app
        /// </summary>
        public void BadRequest()
        {
            Response.StatusCode = 400;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write("Bad Request");
        }

        /// <summary>
        /// Return "unauthorized" to the client app
        /// </summary>
        public void Unauthorized()
        {
            Response.StatusCode = 401;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write("Unauthorized");
        }

        /// <summary>
        /// Return a Json response to the client app
        /// </summary>
        /// <param name="response">The response object to be stringify and return to client app</param>
        public void ReturnJson(JsonResponse response)
        {
            string output = JsonConvert.SerializeObject(response);

            Response.StatusCode = 200;
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/json";
            Response.Write(output);
        }

        /// <summary>
        /// The PageLoad function is to be overriden by child class
        /// In this function it should create a list of handlers and call the HandleRequest function
        /// </summary>
        protected abstract void PageLoad();
    }
}