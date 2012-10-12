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
            : base("settings-module")
        {
            jQuery.Select("#settings-apply-btn").Click(ApplyChange);
            jQuery.Select("#settings-form").Submit(ApplyChange);
        }

        protected override void OnShow()
        {
            jQuery.Select("#settings-language").Value(Settings.UserLanguage);
            jQuery.Select("#settings-sort").Value(Settings.Sort.ToString());
            jQuery.Select("#settings-display-type").Value(Settings.DisplayType.ToString());

            if (Settings.FixAutoResize)
            {
                jQuery.Select("#settings-fix-auto-resize").Attribute("checked", "checked");
            }
            else
            {
                jQuery.Select("#settings-fix-auto-resize").RemoveAttr("checked");
            }
        }

        private void ApplyChange(jQueryEvent e)
        {
            e.PreventDefault();

            Settings.Sort = int.Parse(jQuery.Select("#settings-sort").GetValue(), 10);
            Settings.DisplayType = int.Parse(jQuery.Select("#settings-display-type").GetValue(), 10);
            Settings.FixAutoResize = jQuery.Select("#settings-fix-auto-resize").GetAttribute("checked") == "checked";

            string newLanguage = jQuery.Select("#settings-language").GetValue();
            if (newLanguage != Settings.UserLanguage)
            {
                Settings.UserLanguage = newLanguage;
                Window.Location.Href = "index.html";
            }
        }
    }
}
