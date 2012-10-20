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
        private int copiedMangaMetaId = -1;

        private static AdminMangasModule _instance = null;
        public static AdminMangasModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AdminMangasModule();
                }

                return _instance;
            }
        }

        private AdminMangasModule()
            : base("admin-mangas-module")
        {
            jQuery.Select("#admin-mangas-add-btn").Click(AddButtonClicked);
            jQuery.Select("#admin-mangas-delete-btn").Click(DeleteButtonClicked);
            jQuery.Select("#admin-mangas-edit-btn").Click(EditButtonClicked);
            jQuery.Select("#admin-mangas-refresh-btn").Click(RefreshButtonClicked);
            jQuery.Select("#admin-mangas-copy-meta-btn").Click(CopyMetaButtonClicked);
            jQuery.Select("#admin-mangas-paste-meta-btn").Click(PasteMetaButtonClicked);
            jQuery.Select("#admin-mangas-filter-btn").Click(FilterButtonClicked);
            Utility.FixDropdownTouch(jQuery.Select("#admin-mangas-action-dropdown"));
            pagination = new Pagination(jQuery.Select("#admin-mangas-pagination"), ChangePage, GetTotalPage, "right");
            Refresh();
        }

        protected override void OnShow()
        {
        }

        private void AddButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            AdminMangaAddModal.ShowDialog(this);
        }

        [AlternateSignature]
        public extern void Refresh(JsonResponse response);
        [AlternateSignature]
        public extern void Refresh();
        public void Refresh(bool useFilter, AdminMangaFilterJson filter)
        {
            AdminMangasGetRequest request = new AdminMangasGetRequest();
            if (!Script.IsNullOrUndefined(filter))
            {
                request.filter = filter;
            }

            Request.Send(request, GetRequestSuccess);
            jQuery.Select("#admin-mangas-tbody").Children().Remove();
            Template.Get("admin", "loading-trow", true).AppendTo(jQuery.Select("#admin-mangas-tbody"));
        }

        [AlternateSignature]
        private extern void GetRequestSuccess(JsonResponse response);
        private void GetRequestSuccess(AdminMangasGetResponse response)
        {
            mangas = response.mangas;
            mangas.Sort(delegate(object x, object y)
            {
                return ((MangaJson)y).status - ((MangaJson)x).status;
            });
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
            if (mangas.Length == 0)
            {
                Template.Get("admin", "noitem-trow", true).AppendTo(jQuery.Select("#admin-mangas-tbody"));
            }

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
                jQuery.Select(".admin-mangas-status", row).Text(mangas[i].status == 0 ? Strings.Get("OK") : mangas[i].status == 1 ? Strings.Get("FileMissing") : mangas[i].status == 2 ? Strings.Get("WrongFormat") : Strings.Get("ContentMismatch"));
            }
        }

        private void MetaButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            string idString = jQuery.FromElement(e.Target).GetAttribute("data-id");
            if (!String.IsNullOrEmpty(idString))
            {
                int id = int.Parse(idString, 10);
                AdminMangaMetaModal.ShowDialog(this, id, id);
            }
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

        private void EditButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            int[] ids = GetSelectedIds();

            if (ids.Length > 1)
            {
                ErrorModal.ShowError(Strings.Get("SelectSingleItem"));
            }
            else if (ids.Length == 1)
            {
                AdminMangaEditPathModal.ShowDialog(this, ids[0]);
            }
        }

        private void CopyMetaButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            int[] ids = GetSelectedIds();

            if (ids.Length > 1)
            {
                ErrorModal.ShowError(Strings.Get("SelectSingleItem"));
            }
            else if (ids.Length == 1)
            {
                copiedMangaMetaId = ids[0];
            }
        }

        private void PasteMetaButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();

            if (copiedMangaMetaId != -1)
            {
                int[] ids = GetSelectedIds();

                if (ids.Length > 1)
                {
                    ErrorModal.ShowError(Strings.Get("SelectSingleItem"));
                }
                else if (ids.Length == 1)
                {
                    AdminMangaMetaModal.ShowDialog(this, ids[0], copiedMangaMetaId);
                }
            }
        }

        private void FilterButtonClicked(jQueryEvent e)
        {
            e.PreventDefault();
            AdminMangaFilterModal.ShowDialog(this);
        }
    }
}
