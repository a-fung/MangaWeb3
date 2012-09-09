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
    }
}
