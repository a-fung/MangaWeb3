// InstallingModal.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Install.Module;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Client.Install.Modal
{
    public class InstallingModal : ModalBase
    {
        public InstallingModal()
            : base("install", "installing-modal")
        {
        }

        protected override void Initialize()
        {
            ShowStatic();
        }

        public void SendInstallRequest(InstallRequest request)
        {
            Request.Send(request, InstallRequestSuccess, InstallRequestFailed);
        }

        [AlternateSignature]
        private extern void InstallRequestSuccess(JsonResponse response);
        private void InstallRequestSuccess(InstallResponse response)
        {
            if (response.installsuccessful)
            {
                this.Hide();
                InstallFinishModule.Instance.Show();
            }
            else
            {
                InstallRequestFailed(new Exception("Unknown failure."));
            }
        }

        private void InstallRequestFailed(Exception error)
        {
            this.Hide();
            ErrorModal.ShowError(String.Format(Strings.Get("InstallFailed"), error));
        }
    }
}
