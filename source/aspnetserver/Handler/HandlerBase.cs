using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public abstract class HandlerBase
    {
        public HandlerBase()
        {
        }

        public bool CanHandle(JsonRequest jsonRequest)
        {
            return jsonRequest.type == GetHandleRequestType().Name;
        }

        protected abstract Type GetHandleRequestType();

        public abstract void HandleRequest(string jsonString, AjaxBase ajax);
    }
}