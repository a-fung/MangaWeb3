﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminCollectionEditNameRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminCollectionEditNameRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminCollectionEditNameRequest request = Utility.ParseJson<AdminCollectionEditNameRequest>(jsonString);
            Collection collection = Collection.GetById(request.id);

            if (collection == null)
            {
                ajax.BadRequest();
                return;
            }

            AdminCollectionEditNameResponse response = new AdminCollectionEditNameResponse();

            if (request.name == null || request.name == "")
            {
                response.name = collection.Name;
            }
            else
            {
                request.name = Utility.Remove4PlusBytesUtf8Chars(request.name);

                if (request.name.Length > 50)
                {
                    request.name = request.name.Substring(0, 50);
                }

                if (!Collection.CheckNewCollectionName(request.name))
                {
                    response.status = 1;
                }
                else
                {
                    response.status = 0;
                    collection.Name = request.name;
                    collection.MarkFolderCacheDirty();
                    collection.Save();
                }
            }

            ajax.ReturnJson(response);
        }
    }
}