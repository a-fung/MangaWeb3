// SettingsModule.cs
//

using System;
using System.Collections.Generic;
using System.Html;
using jQueryApi;

namespace afung.MangaWeb3.Client.Module
{
    public class SettingsModule : ClientModuleBase
    {
        private static SettingsModule _instance = null;
        public static SettingsModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsModule();
                }

                return _instance;
            }
        }

        private SettingsModule()
            : base("settings-module", null)
        {
            jQuery.Select("#settings-apply-btn").Click(ApplyChange);
            jQuery.Select("#settings-form").Submit(ApplyChange);

            if (!Environment.IsiOS)
            {
                jQuery.Select("#settings-item-fix-auto-downscale").Hide();
            }

            if (!BootstrapTransition.Support)
            {
                jQuery.Select("#settings-item-animation").Hide();
            }

            if (!Environment.IsKindle)
            {
                jQuery.Select("#settings-item-kindlerefreshdelay").Hide();
            }
        }

        protected override void OnShow()
        {
            jQuery.Select("#settings-language").Value(Settings.UserLanguage);
            jQuery.Select("#settings-sort").Value(Settings.Sort.ToString());
            jQuery.Select("#settings-display-type").Value(Settings.DisplayType.ToString());
            jQuery.Select("#settings-kindlerefreshdelay").Value(Settings.KindleRefreshDelay.ToString());

            if (Settings.FixAutoDownscale)
            {
                jQuery.Select("#settings-fix-auto-downscale").Attribute("checked", "checked");
            }
            else
            {
                jQuery.Select("#settings-fix-auto-downscale").RemoveAttr("checked");
            }

            if (Settings.UseAnimation)
            {
                jQuery.Select("#settings-animation").Attribute("checked", "checked");
            }
            else
            {
                jQuery.Select("#settings-animation").RemoveAttr("checked");
            }
        }

        private void ApplyChange(jQueryEvent e)
        {
            e.PreventDefault();

            Settings.DisplayType = int.Parse(jQuery.Select("#settings-display-type").GetValue(), 10);
            Settings.FixAutoDownscale = jQuery.Select("#settings-fix-auto-downscale").Is(":checked");
            Settings.KindleRefreshDelay = int.Parse(jQuery.Select("#settings-kindlerefreshdelay").GetValue(), 10);

            int newSort = int.Parse(jQuery.Select("#settings-sort").GetValue(), 10);
            if (newSort != Settings.Sort)
            {
                Settings.Sort = newSort;
                MangasModule.Instance.SortItems();
            }

            bool needToRefresh = false;

            string newLanguage = jQuery.Select("#settings-language").GetValue();
            if (newLanguage != Settings.UserLanguage)
            {
                Settings.UserLanguage = newLanguage;
                needToRefresh = true;
            }

            bool newUseAnimation = jQuery.Select("#settings-animation").Is(":checked");
            if (newUseAnimation != Settings.UseAnimation)
            {
                Settings.UseAnimation = newUseAnimation;
                needToRefresh = true;
            }

            if (needToRefresh)
            {
                Window.Location.Href = "index.html";
            }
        }
    }
}
