// SearchModule.cs
//

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using afung.MangaWeb3.Common;
using jQueryApi;

namespace afung.MangaWeb3.Client.Module
{
    public class SearchModule : ClientModuleBase
    {
        private static SearchModule _instance = null;
        public static SearchModule Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SearchModule();
                }

                return _instance;
            }
        }

        private SearchModule()
            : base("search-module", null)
        {
            jQuery.Select("#search-submit").Click(SubmitSearchForm);
            jQuery.Select("#search-form").Submit(SubmitSearchForm);
            Request.Send(new SearchModuleRequest(), SearchModuleRequestSuccess);
        }

        protected override void OnShow()
        {
            jQuery.Select("#search-folder").Value(Settings.SearchFolderSetting.ToString());
        }

        [AlternateSignature]
        private extern void SearchModuleRequestSuccess(JsonResponse response);
        private void SearchModuleRequestSuccess(SearchModuleResponse response)
        {
            foreach (string author in response.authors)
            {
                jQuery.FromHtml("<option></option>").AppendTo(jQuery.Select("#search-author")).Text(author);
            }

            foreach (string series in response.series)
            {
                jQuery.FromHtml("<option></option>").AppendTo(jQuery.Select("#search-series")).Text(series);
            }

            foreach (string publisher in response.publishers)
            {
                jQuery.FromHtml("<option></option>").AppendTo(jQuery.Select("#search-publisher")).Text(publisher);
            }
        }

        private void SubmitSearchForm(jQueryEvent e)
        {
            e.PreventDefault();

            MangaFilter filter = new MangaFilter();
            MangaSearchFilter search = filter.search = new MangaSearchFilter();
            search.title = jQuery.Select("#search-title").GetValue();
            search.folderSetting = Settings.SearchFolderSetting = int.Parse(jQuery.Select("#search-folder").GetValue(), 10);
            search.folder = MangasModule.Instance.CurrentFolder;
            search.author = jQuery.Select("#search-author").GetValue();
            search.series = jQuery.Select("#search-series").GetValue();
            search.publisher = jQuery.Select("#search-publisher").GetValue();
            search.year = int.Parse(jQuery.Select("#search-year").GetValue(), 10);
            if (Number.IsNaN(search.year))
            {
                search.year = -1;
            }

            MangasModule.Instance.Refresh(filter);
        }
    }
}
