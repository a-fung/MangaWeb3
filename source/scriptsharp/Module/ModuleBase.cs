// PageBase.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using System.Runtime.CompilerServices;
using jQueryApi;

namespace afung.MangaWeb3.Client.Module
{
    public abstract class ModuleBase
    {
        private static ModuleBase CurrentModule = null;

        protected jQueryObject attachedObject = null;

        private bool transition;

        [AlternateSignature]
        protected extern ModuleBase(string template, string templateId);
        protected ModuleBase(string template, string templateId, bool transition)
        {
            this.transition = Script.IsNullOrUndefined(transition) ? true : transition;
            attachedObject = Template.Get(template, templateId).AppendTo(jQuery.Select("body")).Hide();
            if (this.transition && Settings.UseAnimation)
            {
                attachedObject.AddClass("fade");
            }
        }

        public void Show(Action callback)
        {
            Action onShow = delegate
            {
                OnShow();
                if (callback != null)
                {
                    callback();
                }
            };

            Action afterHideCurrentModule = delegate
            {
                CurrentModule = null;
                CurrentModule = this;
                OnBeforeShow();
                attachedObject.Show();

                if (transition && Settings.UseAnimation)
                {
                    Window.SetTimeout(
                        delegate
                        {
                            Utility.OnTransitionEnd(attachedObject.AddClass("in"), onShow);
                        },
                        1);
                }
                else
                {
                    onShow();
                }
            };

            if (CurrentModule != null)
            {
                CurrentModule.Hide(afterHideCurrentModule);
            }
            else
            {
                afterHideCurrentModule();
            }
        }

        public void Hide(Action callback)
        {
            Action onHide = delegate
            {
                attachedObject.Hide();
                callback();
            };

            if (transition && Settings.UseAnimation)
            {
                Utility.OnTransitionEnd(attachedObject.RemoveClass("in"), onHide);
            }
            else
            {
                onHide();
            }
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnBeforeShow()
        {
        }
    }
}
