// AdminMangasModule.cs
//

using System;
using System.Collections.Generic;
using afung.MangaWeb3.Client.Admin.Modal;
using jQueryApi;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Common;
using afung.MangaWeb3.Client.Widget;
using afung.MangaWeb3.Client.Modal;

namespace afung.MangaWeb3.Client.Admin.Module
{
    public class AdminMangasModule : AdminModuleBase
    {
        private MangaJson[] mangas;
        private int currentPage;
        private Pagination pagination;

        public AdminMangasModule()
            : base("admin-mangas-module")
        {
        }

        protected override void InnerInitialize()
        {
            jQuery.Select("#admin-mangas-add-btn").Click(AddButtonClicked);
            jQuery.Select("#admin-mangas-delete-btn").Click(DeleteButtonClicked);
            jQuery.Select("#admin-mangas-refresh-btn").Click(RefreshButtonClicked);
            Utility.FixDropdownTouch(jQuery.Select("#admin-mangas-action-dropdown"));
            pagination = new Pagination(jQuery.Select("#admin-collections-pagination"), ChangePage, GetTotalPage, "right");
            Refresh();
        }

        private void AddButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            AdminMangaAddModal.ShowDialog(this);
        }

        [AlternateSignature]
        public extern void Refresh(JsonResponse response);
        public void Refresh()
        {
            Request.Send(new AdminMangasGetRequest(), GetRequestSuccess);
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminMangasGetResponse response)
        {
            mangas = response.mangas;
            ChangePage(1);
            pagination.Refresh(true);
        }

        private int GetTotalPage()
        {
            if (mangas == null || mangas.Length == 0)
            {
                return 1;
            }

            return Math.Ceil(mangas.Length / Environment.ElementsPerPage);
        }

        private void ChangePage(int page)
        {
            currentPage = page;
            jQuery.Select("#admin-mangas-tbody").Children().Remove();
            for (int i = (page - 1) * Environment.ElementsPerPage; i < mangas.Length && i < page * Environment.ElementsPerPage; i++)
            {
                jQueryObject row = Template.Get("admin", "admin-mangas-trow", true).AppendTo(jQuery.Select("#admin-mangas-tbody"));
                jQuery.Select(".admin-mangas-checkbox", row).Value(mangas[i].id.ToString());
                jQuery.Select(".admin-mangas-meta", row).Attribute("data-id", mangas[i].id.ToString()).Click(MetaButtonClicked);
                jQuery.Select(".admin-mangas-title", row).Text(mangas[i].title);
                jQuery.Select(".admin-mangas-collection", row).Text(mangas[i].collection);
                jQuery.Select(".admin-mangas-path", row).Text(mangas[i].path);
                jQuery.Select(".admin-mangas-type", row).Text(mangas[i].type == 0 ? Strings.Get("Zip") : mangas[i].type == 1 ? Strings.Get("RAR") : Strings.Get("PDF"));
                jQuery.Select(".admin-mangas-views", row).Text(mangas[i].view.ToString());
                jQuery.Select(".admin-mangas-status", row).Text(mangas[i].status == 0 ? Strings.Get("OK") : mangas[i].type == 1 ? Strings.Get("FileMissing") : mangas[i].type == 2 ? Strings.Get("WrongFormat") : Strings.Get("ContentMismatch"));
            }
        }

        private void MetaButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
        }

        private void DeleteButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            int[] ids = GetSelectedIds();

            if (ids.Length > 0)
            {
                ConfirmModal.Show(Strings.Get("DeleteMangasConfirm"), DeleteConfirm);
            }
        }

        private int[] GetSelectedIds()
        {
            return Utility.GetSelectedCheckboxIds("admin-mangas-checkbox");
        }

        private void DeleteConfirm(bool confirm)
        {
            int[] ids = GetSelectedIds();

            if (confirm && ids.Length > 0)
            {
                AdminMangasDeleteRequest request = new AdminMangasDeleteRequest();
                request.ids = ids;
                Request.Send(request, Refresh);
            }
        }

        private void RefreshButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            int[] ids = GetSelectedIds();

            if (ids.Length > 0)
            {
                AdminMangasRefreshRequest request = new AdminMangasRefreshRequest();
                request.ids = ids;
                Request.Send(request, Refresh);
            }
        }
    }
}
