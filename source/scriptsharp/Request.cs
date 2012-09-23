// Ajax.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Serialization;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client
{
    public class Request
    {
        public static string Endpoint = "ServerAjax";

        private JsonRequest data;
        private Action<JsonResponse> successCallback;
        private Action<Exception> failureCallback;

        [AlternateSignature]
        public extern static void Send(JsonRequest data);

        [AlternateSignature]
        public extern static void Send(JsonRequest data, Action<JsonResponse> successCallback);

        public static void Send(JsonRequest data, Action<JsonResponse> successCallback, Action<Exception> failureCallback)
        {
            Request request = new Request(data, successCallback, failureCallback);
            request.SendRequest();
        }

        private Request(JsonRequest data, Action<JsonResponse> successCallback, Action<Exception> failureCallback)
        {
            this.data = data;
            this.successCallback = successCallback;
            this.failureCallback = failureCallback;
        }

        private void SendRequest()
        {
            jQueryAjaxOptions options = new jQueryAjaxOptions();
            options.Data = new Dictionary<string, string>("j", Json.Stringify(data));
            options.DataType = "json";
            options.Type = "POST";
            options.Cache = false;
            options.Error = AjaxError;
            options.Success = AjaxSuccess;

            jQuery.Ajax(Endpoint + (Environment.ServerType == ServerType.AspNet ? ".aspx" : ".php"), options);
        }

        private void AjaxError(jQueryXmlHttpRequest request, string textStatus, Exception error)
        {
            if (request.Status == 401)
            {
                // hide all modals first
                ((jQueryBootstrap)jQuery.Select(".modal")).Modal("hide");

                // Show login modal & error message
                LoginModal.GetUserName(
                    delegate(LoginResponse response)
                    {
                        // logged in, retry the request
                        SendRequest();
                    },
                    delegate(Exception error2)
                    {
                        // Login is cancelled, refresh the app
                        Application.Refresh();
                    },
                    true,
                    true);

                // error modal
                ErrorModal.ShowError(Strings.Get("UnauthorizedError"));
            }
            else
            {
                Failure(error);
            }
        }

        private void AjaxSuccess(object responseData, string textStatus, jQueryXmlHttpRequest request)
        {
            if (Script.IsNullOrUndefined(responseData))
            {
                return;
            }

            ErrorResponse error = (ErrorResponse)responseData;
            if (Script.IsValue(error.errorCode) || Script.IsValue(error.errorMsg))
            {
                Failure(new Exception(Strings.Get(error.errorMsg)));
                return;
            }

            Success(responseData);
        }

        private void Failure(Exception error)
        {
            if (Script.IsValue(failureCallback))
            {
                failureCallback(error);
                return;
            }

            ErrorModal.ShowError(String.Format(Strings.Get("RequestFailed"), error));
        }

        private void Success(object responseData)
        {
            if (Script.IsValue(successCallback))
            {
                successCallback((JsonResponse)responseData);
            }
        }
    }

    [Imported]
    public class RequestData
    {
        [ScriptName("j")]
        public string json;
    }
}
