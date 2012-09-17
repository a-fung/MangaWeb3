using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using afung.MangaWeb3.Common;
using afung.MangaWeb3.Server.Handler;
using SevenZip;

namespace afung.MangaWeb3.Server.Install.Handler
{
    public class CheckOtherComponentRequestHandler : HandlerBase
    {
        protected override Type GetHandleRequestType()
        {
            return typeof(CheckOtherComponentRequest);
        }

        public override void HandleRequest(string jsonString, AjaxBase ajax)
        {
            CheckOtherComponentRequest request = Utility.ParseJson<CheckOtherComponentRequest>(jsonString);

            CheckOtherComponentResponse response = new CheckOtherComponentResponse();
            response.pass = false;

            try
            {
                switch (request.component)
                {
                    case 0:
                        SevenZipBase.SetLibraryPath(request.path);
                        SevenZipCompressor c = new SevenZipCompressor();
                        using (System.IO.MemoryStream m1 = new MemoryStream()) using (System.IO.MemoryStream m2 = new MemoryStream())
                        {
                            byte[] b = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                            m1.Write(b, 0, b.Length);
                            c.CompressStream(m1, m2);
                        }
                        response.pass = true;
                        break;
                    case 1:
                    case 2:
                        int exitCode;
                        string output;
                        ProcessLauncher.Run(request.path, Path.Combine(AjaxBase.DirectoryPath, "empty.pdf"), out output, out exitCode);
                        if (exitCode == 0)
                        {
                            if (request.component == 1)
                            {
                                if (output.Contains("Pages"))
                                {
                                    response.pass = true;
                                }
                            }
                            else if (output.Contains("nothing to do"))
                            {
                                response.pass = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }

            ajax.ReturnJson(response);
        }
    }
}