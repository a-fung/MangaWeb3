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

            attachedObject.Bind("webkitTransitionEnd transitionend msTransitionEnd oTransitionEnd", delegate(jQueryEvent e)
            {
                if (e.Target == attachedObject.GetElement(0))
                {
                    OnTransitionEnd();
                }
            });
        }

        protected abstract void Initialize();

        protected virtual void OnTransitionEnd()
        {
        }

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
