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
            string separator = "\\";

            foreach (Collection collection in collections)
            {
                FolderJson folder = new FolderJson();
                folder.name = collection.Name;
                folder.subfolders = new FolderJson[] { };
                folders.Add(folder);
                int collectionPathLength = collection.Path.Length;

                Dictionary<string, FolderJson> folderDictionary = new Dictionary<string, FolderJson>();
                Dictionary<string, object>[] resultSet = Database.Select("manga", "`cid`=" + Database.Quote(collection.Id.ToString()), null, null, "`path`");
                folderDictionary[""] = folder;

                foreach (Dictionary<string, object> result in resultSet)
                {
                    string path = Convert.ToString(result["path"]).Substring(collectionPathLength);
                    int i = 0, j = 0;

                    while ((i = path.IndexOf(separator, j)) != -1)
                    {
                        string relativePath = path.Substring(0, i);
                        if (!folderDictionary.ContainsKey(relativePath.ToLowerInvariant()))
                        {
                            FolderJson subfolder = new FolderJson();
                            subfolder.name = path.Substring(j, i - j);
                            subfolder.subfolders = new FolderJson[] { };
                            folderDictionary[relativePath.ToLowerInvariant()] = subfolder;

                            int k;
                            FolderJson parentFolder = folderDictionary[(k = relativePath.LastIndexOf(separator)) == -1 ? "" : relativePath.Substring(0, k).ToLowerInvariant()];
                            FolderJson[] newSubfolders = new FolderJson[parentFolder.subfolders.Length + 1];
                            Array.Copy(parentFolder.subfolders, newSubfolders, parentFolder.subfolders.Length);
                            newSubfolders[parentFolder.subfolders.Length] = subfolder;
                            parentFolder.subfolders = newSubfolders;
                        }

                        j = i + 1;
                    }
                }
            }

            response.folders = folders.ToArray();
            ajax.ReturnJson(response);
        }
    }
}