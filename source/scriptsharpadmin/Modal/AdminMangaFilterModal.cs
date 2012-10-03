// AdminMangaFilterModal.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Client.Admin.Module;
using afung.MangaWeb3.Client.Modal;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Admin.Modal
{
    public class AdminMangaFilterModal : ModalBase
    {
        private static AdminMangaFilterModal instance = null;

        private static AdminMangasModule mangasModule = null;

        private AdminMangaFilterModal()
            : base("admin", "admin-manga-filter-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#admin-manga-filter-submit").Click(SubmitForm);
            jQuery.Select("#admin-manga-filter-form").Submit(SubmitForm);
        }

        public static void ShowDialog(AdminMangasModule mangasModule)
        {
            if (instance == null)
            {
                instance = new AdminMangaFilterModal();
            }

            instance.InternalShow();
            AdminMangaFilterModal.mangasModule = mangasModule;
        }

        public void InternalShow()
        {
            Request.Send(new AdminMangaFilterRequest(), FilterRequestSuccess);
        }

        [AlternateSignature]
        public extern void FilterRequestSuccess(JsonResponse response);
        public void FilterRequestSuccess(AdminMangaFilterResponse response)
        {
            Show();

            jQueryObject select = jQuery.Select("#admin-manga-filter-collection");
            string value = select.GetValue();
            select.Children().Remove();
            jQuery.FromHtml("<option></option>").AppendTo(select).Value("").Text(Strings.Get("All"));
            foreach (string collection in response.collections)
            {
                jQuery.FromHtml("<option></option>").AppendTo(select).Text(collection);
            }

            select.Value(value);

            select = jQuery.Select("#admin-manga-filter-tag");
            value = select.GetValue();
            select.Children().Remove();
            jQuery.FromHtml("<option></option>").AppendTo(select).Value("").Text(Strings.Get("All"));
            foreach (string tag in response.tags)
            {
                jQuery.FromHtml("<option></option>").AppendTo(select).Text(tag);
            }

            select.Value(value);

            select = jQuery.Select("#admin-manga-filter-author");
            value = select.GetValue();
            select.Children().Remove();
            jQuery.FromHtml("<option></option>").AppendTo(select).Value("").Text(Strings.Get("All"));
            foreach (string author in response.authors)
            {
                jQuery.FromHtml("<option></option>").AppendTo(select).Text(author);
            }

            select.Value(value);
        }

        private void SubmitForm(jQueryEvent e)
        {
            e.PreventDefault();
            Hide();
            AdminMangaFilterJson filter = new AdminMangaFilterJson();
            filter.collection = jQuery.Select("#admin-manga-filter-collection").GetValue();
            filter.tag = jQuery.Select("#admin-manga-filter-tag").GetValue();
            filter.author = jQuery.Select("#admin-manga-filter-author").GetValue();
            filter.type = int.Parse(jQuery.Select("#admin-manga-filter-type").GetValue(), 10);
            if (Number.IsNaN(filter.type))
            {
                filter.type = -1;
            }

            mangasModule.Refresh(true, filter);
        }
    }
}
