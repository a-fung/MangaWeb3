// AdminMangaMetaModal.cs
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
    public class AdminMangaMetaModal : ModalBase
    {
        private static AdminMangaMetaModal instance = null;

        private static AdminMangasModule mangasModule = null;

        private bool submittingForm = false;
        private int mangaId;

        private AdminMangaMetaModal()
            : base("admin", "admin-manga-meta-modal")
        {
        }

        protected override void Initialize()
        {
            jQuery.Select("#admin-manga-meta-submit").Click(SubmitForm);
            jQuery.Select("#admin-manga-meta-form").Submit(SubmitForm);
        }

        protected override void OnShown()
        {
            jQuery.Select("#admin-manga-meta-title").Focus();
        }

        public static void ShowDialog(AdminMangasModule mangasModule, int id, int copiedId)
        {
            if (instance == null)
            {
                instance = new AdminMangaMetaModal();
            }

            instance.InternalShow(id, copiedId);
            AdminMangaMetaModal.mangasModule = mangasModule;
        }

        public void InternalShow(int id, int copiedId)
        {
            AdminMangaMetaGetRequest request = new AdminMangaMetaGetRequest();
            mangaId = id;
            request.id = copiedId;

            Request.Send(request, GetMetaSuccess);
        }

        [AlternateSignature]
        private extern void GetMetaSuccess(JsonResponse response);
        private void GetMetaSuccess(AdminMangaMetaGetResponse response)
        {
            Show();
            jQuery.Select("#admin-manga-meta-title").Value(response.meta.title).Focus();

            jQuery.Select("#admin-manga-meta-volume").Value(response.meta.volume < 0 ? "" : response.meta.volume.ToString());
            jQuery.Select("#admin-manga-meta-year").Value(response.meta.year < 0 ? "" : response.meta.year.ToString());

            jQuery.Select("#admin-manga-meta-tags").Value(response.meta.tags.Join(", "));

            ((BootstrapTypeahead)((jQueryBootstrap)jQuery.Select("#admin-manga-meta-author").Value(response.meta.author)).Typeahead().GetDataValue("typeahead")).Source = response.authors;
            ((BootstrapTypeahead)((jQueryBootstrap)jQuery.Select("#admin-manga-meta-series").Value(response.meta.series)).Typeahead().GetDataValue("typeahead")).Source = response.series;
            ((BootstrapTypeahead)((jQueryBootstrap)jQuery.Select("#admin-manga-meta-publisher").Value(response.meta.publisher)).Typeahead().GetDataValue("typeahead")).Source = response.publishers;
        }

        private void SubmitForm(jQueryEvent e)
        {
            e.PreventDefault();

            if (!submittingForm)
            {
                AdminMangaMetaEditRequest request = new AdminMangaMetaEditRequest();
                request.id = mangaId;
                request.meta = new AdminMangaMetaJson();
                request.meta.author = jQuery.Select("#admin-manga-meta-author").GetValue();
                request.meta.title = jQuery.Select("#admin-manga-meta-title").GetValue();
                request.meta.series = jQuery.Select("#admin-manga-meta-series").GetValue();
                request.meta.publisher = jQuery.Select("#admin-manga-meta-publisher").GetValue();

                int volume = int.Parse(jQuery.Select("#admin-manga-meta-volume").GetValue());
                if (Number.IsNaN(volume) || volume < 0) volume = -1;
                request.meta.volume = volume;

                int year = int.Parse(jQuery.Select("#admin-manga-meta-year").GetValue());
                if (Number.IsNaN(year) || year < 0) year = -1;
                request.meta.year = year;

                List<string> tags = (List<string>)(object)jQuery.Select("#admin-manga-meta-tags").GetValue().Split(",");
                for (int i = 0; i < tags.Count; )
                {
                    tags[i] = tags[i].Trim();
                    if (tags[i] == "")
                    {
                        tags.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                request.meta.tags = (string[])(object)tags;

                submittingForm = true;

                Request.Send(request, SubmitSuccess, SubmitFailure);
            }
        }

        [AlternateSignature]
        private extern void SubmitSuccess(JsonResponse response);
        private void SubmitSuccess()
        {
            submittingForm = false;

            Hide();
            mangasModule.Refresh();
        }

        private void SubmitFailure(Exception error)
        {
            submittingForm = false;

            ErrorModal.ShowError(error.ToString());
        }
    }
}
