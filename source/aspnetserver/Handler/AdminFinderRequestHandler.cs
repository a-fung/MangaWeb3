using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using afung.MangaWeb3.Common;

namespace afung.MangaWeb3.Server.Handler
{
    public class AdminFinderRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(AdminFinderRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            if (!User.IsAdminLoggedIn(ajax))
            {
                ajax.Unauthorized();
                return;
            }

            AdminFinderRequest request = Utility.ParseJson<AdminFinderRequest>(jsonString);
            AdminFinderResponse response = new AdminFinderResponse();
            Collection collection = null;
            string[][] finderData;

            if (request.cid != -1)
            {
                collection = Collection.GetById(request.cid);
                if (collection == null)
                {
                    ajax.BadRequest();
                    return;
                }

                finderData = new string[][] { new string[] { collection.Name, collection.Path } };
            }
            else
            {
                List<string[]> diskData = new List<string[]>();
                string[] discs = Environment.GetLogicalDrives();
                for (int i = 0; i < discs.Length; i++)
                {
                    string driveLetter = discs[i].Replace(":\\", "");
                    using (System.Management.ManagementObject diskObj = new System.Management.ManagementObject("win32_logicaldisk.deviceid=\"" + driveLetter + ":\""))
                    {
                        if (diskObj["DriveType"].ToString() == "3") // Local Disk
                        {
                            diskData.Add(new string[] { driveLetter + ":", driveLetter + ":\\" });
                        }
                    }
                }

                finderData = diskData.ToArray();
            }

            Random r = new Random();
            StringBuilder tokenBuilder = new StringBuilder();
            for (int i = 0; i < 32; i++)
            {
                int n = r.Next(36);
                if (n < 10)
                {
                    tokenBuilder.Append(n);
                }
                else
                {
                    tokenBuilder.Append(Convert.ToChar(55 + n));
                }
            }

            response.token = tokenBuilder.ToString();

            SessionWrapper.SetFinderData(ajax, response.token, finderData);

            ajax.ReturnJson(response);
        }
    }
}