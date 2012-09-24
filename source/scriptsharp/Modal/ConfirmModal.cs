// ConfirmModal.cs
//

using System;
using System.Collections.Generic;
using jQueryApi;

namespace afung.MangaWeb3.Client.Modal
{
    public class ConfirmModal : ModalBase
    {
        private static ConfirmModal instance = null;
        private static Action<bool> callback = null;

        private ConfirmModal()
            : base("client", "confirm-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#confirm-modal-yes-btn").Click(YesButtonClicked);
            jQuery.Select("#confirm-modal-no-btn").Click(NoButtonClicked);
        }

        public static void Show(string message, Action<bool> callback)
        {
            if (instance == null)
            {
                instance = new ConfirmModal();
            }
            ConfirmModal.callback = callback;
            instance.InternalShow(message);
        }

        public void InternalShow(string message)
        {
            jQuery.Select("#confirm-modal-msg").Text(message);
            ShowStatic();
        }

        private void InternalButtonClicked(bool confirm)
        {
            Hide();
            callback(confirm);
        }

        private void YesButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            InternalButtonClicked(true);
        }

        private void NoButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            InternalButtonClicked(false);
        }
    }
}
