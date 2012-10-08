using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace afung.MangaWeb3.Server.Provider
{
    public class ImageProvider
    {
        private static ImageCodecInfo _imageEncoder = null;
        private static ImageCodecInfo ImageEncoder
        {
            get
            {
                if (_imageEncoder == null)
                {
                    foreach (ImageCodecInfo c in ImageCodecInfo.GetImageEncoders())
                    {
                        if (c.FormatDescription.Equals("jpeg", StringComparison.InvariantCultureIgnoreCase))
                        {
                            _imageEncoder = c;
                            break;
                        }
                    }
                }
                return _imageEncoder;
            }
        }
        private static EncoderParameters _encoderParams = null;
        private static EncoderParameters EncoderParams
        {
            get
            {
                if (_encoderParams == null)
                {
                    _encoderParams = new EncoderParameters(1);
                    _encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 90L);
                }
                return _encoderParams;
            }
        }

        public static void ResizeFile(string inputFile, string outputFile, int width, int height)
        {
            int part = -1;
            using (Image image = Image.FromFile(inputFile))
            {
                using (Bitmap resizedImage = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(resizedImage))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                        g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                        if (part == -1)
                        {
                            g.DrawImage(image, 0, 0, resizedImage.Width, resizedImage.Height);
                        }
                        else
                        {
                            g.DrawImage(image, part == 0 ? 0 : -resizedImage.Width, 0, resizedImage.Width * 2, resizedImage.Height);
                        }
                    }

                    using (FileStream outputFileStream = File.Open(outputFile, FileMode.Create))
                    {
                        resizedImage.Save(outputFileStream, ImageEncoder, EncoderParams);
                    }
                }
            }
        }
    }
}