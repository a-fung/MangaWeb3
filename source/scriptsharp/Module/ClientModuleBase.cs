// MangaModuleBase.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Widget;
using jQueryApi;

namespace afung.MangaWeb3.Client.Module
{
    public abstract class ClientModuleBase : ModuleBase
    {
        protected ClientModuleBase(string templateId, Action loginRefreshCallback)
            : base("client", templateId)
        {
            jQuery.Select(".nav-mangas", attachedObject).Click(NavMangasClicked);
            jQuery.Select(".nav-folders", attachedObject).Click(NavFoldersClicked);
            jQuery.Select(".nav-search", attachedObject).Click(NavSearchClicked);
            jQuery.Select(".nav-settings", attachedObject).Click(NavSettingsClicked);
            new LoginWidget(jQuery.Select(".nav-main", attachedObject), loginRefreshCallback);
        }

        private void NavMangasClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (this.GetType() != typeof(MangasModule))
            {
                MangasModule.Instance.Show(null);
            }
        }

        private void NavFoldersClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (this.GetType() != typeof(FoldersModule))
            {
                FoldersModule.Instance.Show(null);
            }
        }

        private void NavSearchClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (this.GetType() != typeof(SearchModule))
            {
                SearchModule.Instance.Show(null);
            }
        }

        private void NavSettingsClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (this.GetType() != typeof(SettingsModule))
            {
                SettingsModule.Instance.Show(null);
            }
        }
    }
}
