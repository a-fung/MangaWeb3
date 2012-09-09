// PageBase.cs
//

using System;
using System.Collections.Generic;
using jQueryApi;

namespace afung.MangaWeb3.Client.Module
{
    public abstract class ModuleBase : IDisposable
    {
        private static ModuleBase CurrentModule = null;

        protected jQueryObject attachedObject = null;

        public ModuleBase(string template, string templateId)
        {
            if (CurrentModule != null)
            {
                CurrentModule.Dispose();
            }

            CurrentModule = null;
            CurrentModule = this;

            attachedObject = Template.Get(template, templateId).AppendTo(jQuery.Select(HtmlConstants.TagBody));

            Initialize();
        }

        protected abstract void Initialize();

        public void Dispose()
        {
            attachedObject.Remove();
        }
    }
}
