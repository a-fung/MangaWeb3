// AdminStackTraceModal.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Modal;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Modal
{
    public class AdminStackTraceModal : ModalBase
    {
        private static AdminStackTraceModal instance = null;

        private AdminStackTraceModal()
            : base("admin", "admin-stacktrace-modal")
        {
        }

        protected override void Initialize()
        {
        }

        public static void ShowDialog(string stackTrace)
        {
            if (instance == null)
            {
                instance = new AdminStackTraceModal();
            }

            instance.InternalShow(stackTrace);
        }

        public void InternalShow(string stackTrace)
        {
            jQuery.Select("#admin-stacktrace-body").Text(stackTrace);
            Show();
        }
    }
}
