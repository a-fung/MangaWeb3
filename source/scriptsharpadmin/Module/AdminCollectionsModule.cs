// AdminCollectionsModule.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Admin.Modal;
using afung.MangaWeb3.Client.Widget;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Module
{
    public class AdminCollectionsModule : AdminModuleBase
    {
        private CollectionJson[] collections;
        private int currentPage;
        private Pagination pagination;

        public AdminCollectionsModule()
            : base("admin-collections-module")
        {
        }

        protected override void InnerInitialize()
        {
            jQuery.Select("#admin-collections-add-btn").Click(AddButtonClicked);
            pagination = new Pagination(jQuery.Select("#admin-collections-pagination"), ChangePage, GetTotalPage, "right");
            Refresh();
        }

        private void AddButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            AdminCollectionAddModal.ShowDialog();
        }

        public void Refresh()
        {
            Request.Send(new AdminCollectionsGetRequest(), GetRequestSuccess);
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminCollectionsGetResponse response)
        {
            collections = response.collections;
            ChangePage(1);
            pagination.Refresh();
        }

        private void ChangePage(int page)
        {
            currentPage = page;
            jQuery.Select("#admin-collections-tbody").Children().Remove();
            for (int i = (page - 1) * Environment.ElementsPerPage; i < collections.Length && i < page * Environment.ElementsPerPage; i++)
            {
                jQueryObject row = Template.Get("admin", "admin-collections-trow", true).AppendTo(jQuery.Select("#admin-collections-tbody"));
                jQuery.Select(".admin-collections-checkbox", row).Value(collections[i].id.ToString());
                jQuery.Select(".admin-collections-users", row).Attribute("data-id", collections[i].id.ToString()).Click(UsersButtonClicked);
                jQuery.Select(".admin-collections-name", row).Text(collections[i].name);
                jQuery.Select(".admin-collections-path", row).Text(collections[i].path);
                jQuery.Select(".admin-collections-public", row).Text(Strings.Get(collections[i].public_ ? "Yes" : "No"));
                jQuery.Select(".admin-collections-autoadd", row).Text(Strings.Get(collections[i].autoadd ? "Yes" : "No"));
            }
        }

        private void UsersButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
        }

        private int GetTotalPage()
        {
            if (collections == null || collections.Length == 0)
            {
                return 1;
            }

            return Math.Ceil(collections.Length / Environment.ElementsPerPage);
        }
    }
}
