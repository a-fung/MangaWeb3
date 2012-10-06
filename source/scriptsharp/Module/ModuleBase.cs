// PageBase.cs
//

using System;
using System.Collections.Generic;
using jQueryApi;

namespace afung.MangaWeb3.Client.Module
{
    public abstract class ModuleBase
    {
        private static ModuleBase CurrentModule = null;

        protected jQueryObject attachedObject = null;

        protected ModuleBase(string template, string templateId)
        {
            attachedObject = Template.Get(template, templateId).AppendTo(jQuery.Select("body")).Hide();
        }

        public void Show()
        {
            if (CurrentModule != null)
            {
                CurrentModule.Hide();
            }

            CurrentModule = null;
            CurrentModule = this;
            attachedObject.Show();
            OnShow();
        }

        public void Hide()
        {
            attachedObject.Hide();
        }

        protected abstract void OnShow();
    }
}
