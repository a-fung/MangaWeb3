using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class MangaListItemDetailsRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(MangaListItemDetailsRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            MangaListItemDetailsRequest request = Utility.ParseJson<MangaListItemDetailsRequest>(jsonString);
            Manga manga = Manga.GetById(request.id);

            if (manga == null || !manga.ParentCollection.Accessible(ajax))
            {
                ajax.BadRequest();
                return;
            }

            MangaListItemDetailsResponse response = new MangaListItemDetailsResponse();
            response.author = manga.Meta.Author;
            response.series = manga.Meta.Series;
            response.volume = manga.Meta.Volume;
            response.year = manga.Meta.Year;
            response.publisher = manga.Meta.Publisher;
            response.tags = manga.GetTags();
            ajax.ReturnJson(response);
        }
    }
}