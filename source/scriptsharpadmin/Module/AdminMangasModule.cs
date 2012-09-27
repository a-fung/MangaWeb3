// AdminMangasModule.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Admin.Modal;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Module
{
    public class AdminMangasModule : AdminModuleBase
    {
        public AdminMangasModule()
            : base("admin-mangas-module")
        {
        }

        protected override void InnerInitialize()
        {
            jQuery.Select("#admin-mangas-add-btn").Click(AddButtonClicked);
        }

        private void AddButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            AdminMangaAddModal.ShowDialog(this);
        }
    }
}
