using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server.Provider
{
    public class PdfProvider : IMangaProvider
    {
        public const string Extension = ".pdf";

        public PdfProvider()
        {
            if (!Settings.UsePdf)
            {
                throw new InvalidOperationException("PDF format is not configured to use.");
            }
        }

        private int GetNumberOfPages(string path)
        {
            string output;
            int exitCode;

            ProcessLauncher.Run(Config.PdfinfoPath, path, out output, out exitCode);

            if (exitCode == 0)
            {
                int index = output.IndexOf("Pages:");
                if (index != -1)
                {
                    int index2 = output.IndexOf('\n', index);
                    return int.Parse(output.Substring(index + 6, index2 - index - 6));
                }
            }

            return 0;
        }

        public bool TryOpen(string path)
        {
            return GetNumberOfPages(path) > 0;
        }
    }
}