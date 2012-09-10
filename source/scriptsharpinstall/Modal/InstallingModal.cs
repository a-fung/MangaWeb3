// InstallingModal.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Client.Install.Modal
{
    public class InstallingModal : ModalBase
    {
        private InstallRequest request;

        public InstallingModal(InstallRequest request)
            : base("install", "installing-modal")
        {
            this.request = request;
        }

        protected override void Initialize()
        {
            ShowStatic();

            Request.Send(request, InstallRequestSuccess);
        }

        [AlternateSignature]
        private extern void InstallRequestSuccess(JsonResponse response);
        private void InstallRequestSuccess()
        {
        }
    }
}
