using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class SearchModuleRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(SearchModuleRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            SearchModuleResponse response = new SearchModuleResponse();

            string mangaWhere = "`status`='0'";
            User user = User.GetCurrentUser(ajax);
            string collectionSelect = "FALSE";
            if (Settings.AllowGuest || user != null)
            {
                collectionSelect += " OR `cid` IN (SELECT `id` FROM `collection` WHERE `public`='1')";
            }

            if (user != null)
            {
                collectionSelect += " OR `cid` IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(user.Id.ToString()) + " AND `access`='1')";
                mangaWhere += " AND `cid` NOT IN (SELECT `cid` FROM `collectionuser` WHERE `uid`=" + Database.Quote(user.Id.ToString()) + " AND `access`='0')";
            }

            mangaWhere += " AND (" + collectionSelect + ")";

            string where = "`mid` IN (SELECT `id` FROM `manga` WHERE " + mangaWhere + ")";

            response.authors = Database.GetDistinctStringValues("meta", "author", where);
            response.series = Database.GetDistinctStringValues("meta", "series", where);
            response.publishers = Database.GetDistinctStringValues("meta", "publisher", where);

            ajax.ReturnJson(response);
        }
    }
}