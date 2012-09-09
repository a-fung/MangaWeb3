// ErrorModal.cs
//

using System;
using System.Collections.Generic;
using jQueryApi;

namespace afung.MangaWeb3.Client.Modal
{
    public class ErrorModal : ModalBase
    {
        private static ErrorModal instance = null;

        private ErrorModal()
            : base("client", "error-modal")
        {
        }

        protected override void Initialize()
        {
        }

        public static void ShowError(string errorMsg)
        {
            if (instance == null)
            {
                instance = new ErrorModal();
            }
            instance.InternalShowError(errorMsg);
        }

        public void InternalShowError(string errorMsg)
        {
            jQuery.Select("#error-modal-msg", attachedObject).Text(errorMsg);
            Show();
        }
    }
}
