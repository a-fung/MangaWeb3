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
            attachedObject = Template.Get(template, templateId).AppendTo(jQuery.Select(HtmlConstants.TagBody));

            Initialize();
        }

        protected abstract void Initialize();

        protected void Show()
        {
            ((jQueryBootstrap)attachedObject).Modal();
        }

        protected void ShowStatic()
        {
            ((jQueryBootstrap)attachedObject).Modal(
                new Dictionary<string, object>(
                    "backdrop",
                    "static"));
        }

        protected void Hide()
        {
            ((jQueryBootstrap)attachedObject).Modal("hide");
        }
    }
}
