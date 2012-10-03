// ModalBase.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace afung.MangaWeb3.Client.Modal
{
    public abstract class ModalBase
    {
        protected jQueryObject attachedObject = null;

        public ModalBase(string template, string templateId)
        {
            attachedObject = Template.Get(template, templateId).AppendTo(jQuery.Select("body"));

            Initialize();

            attachedObject.Bind("shown", OnShown);
        }

        protected abstract void Initialize();

        [AlternateSignature]
        protected extern void OnShown(jQueryEvent e);
        protected virtual void OnShown()
        {
        }

        protected void Show()
        {
            ((jQueryBootstrap)attachedObject).Modal(
                new Dictionary<string, object>(
                    "keyboard",
                    false));
            FixVerticalCenter();
        }

        protected void ShowStatic()
        {
            ((jQueryBootstrap)attachedObject).Modal(
                new Dictionary<string, object>(
                    "backdrop",
                    "static",
                    "keyboard",
                    false));
            FixVerticalCenter();
        }

        protected void Hide()
        {
            ((jQueryBootstrap)attachedObject).Modal("hide");
        }

        private void FixVerticalCenter()
        {
            attachedObject.CSS(
                new Dictionary<string, object>(
                    "margin-top",
                    attachedObject.GetOuterHeight() / -2));
        }
    }
}
