using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManipulation
{
    public enum ImageFormat
    {
        WithAlpha,NoAlpha
    }

    public static class ImageFormatConverter
    {
        public static Bitmap ConvertTo32Bpp(Bitmap bmp,ImageFormat format)
        {
            Bitmap bm;
            switch (format)
            {
                case ImageFormat.WithAlpha:
                    bm = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);break;
                case ImageFormat.NoAlpha:
                    bm = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb); break;
                default: bm = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb); break;
            }            
            bm.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);           
            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(bmp, 0, 0);
            g.Dispose();
            return bm;
        }
    }
}
