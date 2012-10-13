using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class MangaListRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(MangaListRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            MangaListRequest request = Utility.ParseJson<MangaListRequest>(jsonString);
            MangaListResponse response = new MangaListResponse();
            response.items = Manga.ToListItemJsonArray(Manga.GetMangaList(ajax, request.filter));

            if (request.filter == null ||
                (
                    (request.filter.folder == null || request.filter.folder == "") &&
                    (request.filter.tag == null || request.filter.tag == "") &&
                    request.filter.search == null
                ))
            {
                ThreadHelper.Run("ProcessAutoAddStage1");
            }

            ajax.ReturnJson(response);
        }
    }
}