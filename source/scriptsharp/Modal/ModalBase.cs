// ModalBase.cs
//

using System;
using System.Collections.Generic;
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
        }

        protected abstract void Initialize();

        protected void Show()
        {
            ((jQueryBootstrap)attachedObject).Modal();
            FixVerticalCenter();
        }

        protected void ShowStatic()
        {
            ((jQueryBootstrap)attachedObject).Modal(
                new Dictionary<string, object>(
                    "backdrop",
                    "static"));
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
