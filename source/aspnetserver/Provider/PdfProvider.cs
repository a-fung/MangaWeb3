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

            ProcessLauncher.Run(Config.PdfinfoPath, "\"" + path + "\"", out output, out exitCode);

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

        public string[] GetContent(string path)
        {
            int pages = GetNumberOfPages(path);
            string[] content = new string[pages];
            for (int p = 0; p < pages; p++)
            {
                content[p] = (p + 1).ToString();
            }

            return content;
        }

        public string OutputFile(string path, string page, string outputPath)
        {
            int pageInt, numberOfPages = -1;
            if (!int.TryParse(page, out pageInt) || pageInt < 1 || pageInt > (numberOfPages = GetNumberOfPages(path)))
            {
                InvalidOperationException exception = new InvalidOperationException("Read PDF file error");
                exception.Data["manga_status"] = System.IO.File.Exists(path) ? (numberOfPages == 0 ? 2 : 3) : 1;
                throw exception;
            }

            string output;
            int exitCode;
            outputPath = outputPath + ".png";

            ProcessLauncher.Run(Config.MudrawPath, "-o \"" + outputPath + "\" -r 300 \"" + path + "\" " + page, out output, out exitCode);

            if (exitCode != 0)
            {
                InvalidOperationException exception = new InvalidOperationException("Render PDF file error");
                exception.Data["manga_status"] = 2;
                throw exception;
            }

            return outputPath;
        }
    }
}