using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Forms;

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
            try
            {
                using (PdfFile pdfFile = new PdfFile(path))
                {
                    lock (pdfFile.Wrapper)
                    {
                        return pdfFile.Wrapper.PageCount;
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
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
                exception.Data["manga_status"] = File.Exists(path) ? (numberOfPages == 0 ? 2 : 3) : 1;
                throw exception;
            }

            outputPath = outputPath + ".png";

            using (PdfFile pdfFile = new PdfFile(path))
            {
                lock (pdfFile.Wrapper)
                {
                    pdfFile.Wrapper.CurrentPage = pageInt;
                    using (PictureBox pictureBox = new PictureBox())
                    {
                        pictureBox.Width = pictureBox.Height = 3000;
                        pdfFile.Wrapper.FitToHeight(pictureBox.Handle);
                        pictureBox.Width = pdfFile.Wrapper.PageWidth;
                        pdfFile.Wrapper.RenderPage(pictureBox.Handle);

                        using (Bitmap image = new Bitmap(pdfFile.Wrapper.PageWidth, pdfFile.Wrapper.PageHeight))
                        {
                            using (Graphics g = Graphics.FromImage(image))
                            {
                                pdfFile.Wrapper.ClientBounds = new Rectangle(0, 0, pdfFile.Wrapper.PageWidth, pdfFile.Wrapper.PageHeight);
                                pdfFile.Wrapper.CurrentX = pdfFile.Wrapper.CurrentY = 0;
                                IntPtr ptr = g.GetHdc();
                                pdfFile.Wrapper.DrawPageHDC(ptr);
                                g.ReleaseHdc(ptr);
                            }

                            using (FileStream outputFile = new FileStream(outputPath, FileMode.Create))
                            {
                                image.Save(outputFile, ImageFormat.Png);
                            }
                        }
                    }
                }
            }

            return outputPath;
        }
    }
}