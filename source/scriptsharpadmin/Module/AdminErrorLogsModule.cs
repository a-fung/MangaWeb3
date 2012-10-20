// AdminErrorLogsModule.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Widget;
using afung.MangaWeb3.Common;
using jQueryApi;
using afung.MangaWeb3.Client.Admin.Modal;

namespace afung.MangaWeb3.Client.Admin.Module
{
    public class AdminErrorLogsModule : AdminModuleBase
    {
        private Pagination pagination;

        private static AdminErrorLogsModule _instance = null;
        public static AdminErrorLogsModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AdminErrorLogsModule();
                }

                return _instance;
            }
        }

        private int currentPage = 1;
        private int numberOfLogs = 0;

        private AdminErrorLogsModule()
            : base("admin-errorlogs-module")
        {
            pagination = new Pagination(jQuery.Select("#admin-errorlogs-pagination"), ChangePage, GetTotalPage, "centered");
        }

        protected override void OnShow()
        {
            ChangePage(currentPage);
        }

        private int GetTotalPage()
        {
            return Math.Ceil(numberOfLogs / Environment.ElementsPerPage);
        }

        private void ChangePage(int page)
        {
            currentPage = page;

            AdminErrorLogsGetRequest request = new AdminErrorLogsGetRequest();
            request.page = page - 1;
            request.elementsPerPage = Environment.ElementsPerPage;
            Request.Send(request, GetRequestSuccess);
            jQuery.Select("#admin-errorlogs-tbody").Children().Remove();
            Template.Get("admin", "loading-trow", true).AppendTo(jQuery.Select("#admin-errorlogs-tbody"));
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminErrorLogsGetResponse response)
        {
            numberOfLogs = response.numberOfLogs;
            pagination.Refresh();

            jQuery.Select("#admin-errorlogs-tbody").Children().Remove();
            if (response.logs.Length == 0)
            {
                Template.Get("admin", "noitem-trow", true).AppendTo(jQuery.Select("#admin-errorlogs-tbody"));
            }

            for (int i = 0; i < response.logs.Length; i++)
            {
                jQueryObject row = Template.Get("admin", "admin-errorlogs-trow", true).AppendTo(jQuery.Select("#admin-errorlogs-tbody"));
                jQuery.Select(".admin-errorlogs-time", row).Text(String.Format(Strings.Get("TimeFormat"), new Date(response.logs[i].time * 1000)));
                jQuery.Select(".admin-errorlogs-stacktrace", row).Attribute("data-stacktrace", response.logs[i].stackTrace).Click(ShowButtonClicked);
                jQuery.Select(".admin-errorlogs-type", row).Text(response.logs[i].type);
                jQuery.Select(".admin-errorlogs-source", row).Text(response.logs[i].source);
                jQuery.Select(".admin-errorlogs-message", row).Text(response.logs[i].message);
            }
        }

        private void ShowButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            AdminStackTraceModal.ShowDialog(jQuery.FromElement(e.Target).GetAttribute("data-stacktrace"));
        }
    }
}
