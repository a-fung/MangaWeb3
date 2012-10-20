// AdminSettingsModal.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Modal
{
    public class AdminSettingsModal : ModalBase
    {
        private static AdminSettingsModal instance = null;

        private AdminSettingsModal()
            : base("admin", "admin-settings-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#admin-settings-submit").Click(SubmitForm);
            jQuery.Select("#admin-settings-form").Submit(SubmitForm);
        }

        protected override void OnShown()
        {
            jQuery.Select("#admin-settings-guest").Focus();
        }

        public static void ShowDialog()
        {
            if (instance == null)
            {
                instance = new AdminSettingsModal();
            }
            instance.InternalShow();
        }

        public void InternalShow()
        {
            Request.Send(new AdminSettingsGetRequest(), GetRequestSuccess);
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminSettingsGetResponse response)
        {
            Show();

            if (response.guest)
            {
                jQuery.Select("#admin-settings-guest").Attribute("checked", "checked");
            }
            else
            {
                jQuery.Select("#admin-settings-guest").RemoveAttr("checked");
            }

            if (response.zip)
            {
                jQuery.Select("#admin-settings-zip").Attribute("checked", "checked");
            }
            else
            {
                jQuery.Select("#admin-settings-zip").RemoveAttr("checked");
            }

            if (response.rar)
            {
                jQuery.Select("#admin-settings-rar").Attribute("checked", "checked");
            }
            else
            {
                jQuery.Select("#admin-settings-rar").RemoveAttr("checked");
            }

            if (response.pdf)
            {
                jQuery.Select("#admin-settings-pdf").Attribute("checked", "checked");
            }
            else
            {
                jQuery.Select("#admin-settings-pdf").RemoveAttr("checked");
            }

            jQuery.Select("#admin-settings-preprocess-count").Value(response.preprocessCount.ToString());
            jQuery.Select("#admin-settings-preprocess-delay").Value(response.preprocessDelay.ToString());
            jQuery.Select("#admin-settings-cachelimit").Value(response.cacheLimit.ToString());
        }

        private void SubmitForm(jQueryEvent e)
        {
            e.PreventDefault();

            AdminSettingsSetRequest request = new AdminSettingsSetRequest();
            request.guest = jQuery.Select("#admin-settings-guest").GetAttribute("checked") == "checked";
            request.zip = jQuery.Select("#admin-settings-zip").GetAttribute("checked") == "checked";
            request.rar = jQuery.Select("#admin-settings-rar").GetAttribute("checked") == "checked";
            request.pdf = jQuery.Select("#admin-settings-pdf").GetAttribute("checked") == "checked";
            request.preprocessCount = Number.IsFinite(request.preprocessCount = int.Parse(jQuery.Select("#admin-settings-preprocess-count").GetValue())) ? request.preprocessCount : -1;
            request.preprocessDelay = Number.IsFinite(request.preprocessDelay = int.Parse(jQuery.Select("#admin-settings-preprocess-delay").GetValue())) ? request.preprocessDelay : -1;
            request.cacheLimit = Number.IsFinite(request.cacheLimit = int.Parse(jQuery.Select("#admin-settings-cachelimit").GetValue())) ? request.cacheLimit : -1;

            Request.Send(
                request,
                delegate(JsonResponse response)
                {
                    Hide();
                });
        }
    }
}
