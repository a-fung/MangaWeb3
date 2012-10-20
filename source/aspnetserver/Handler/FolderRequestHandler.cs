using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class FolderRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(FolderRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            FolderResponse response = new FolderResponse();
            Collection[] collections = Collection.GetAccessible(ajax);
            List<FolderJson> folders = new List<FolderJson>();

            bool ready = true;
            foreach (Collection collection in collections)
            {
                string folderJsonString = collection.FolderCache;
                if (folderJsonString != null)
                {
                    folders.Add(Utility.ParseJson<FolderJson>(folderJsonString));
                }
                else
                {
                    ready = false;

                    if (collection.CacheStatus == 1)
                    {
                        ThreadHelper.Run("CollectionProcessFolderCache", collection.Id);
                    }
                }
            }

            if (!ready)
            {
                response.status = 1;
            }
            else
            {
                response.status = 0;
                response.folders = folders.ToArray();
            }

            ajax.ReturnJson(response);
        }
    }
}