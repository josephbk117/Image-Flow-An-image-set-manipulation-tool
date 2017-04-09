using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageManipulation
{
    public class ImageResize : IMageManipulation
    {
        Bitmap bmp;
        int width, height;
        public ImageResize(Bitmap bitmap, int newWidth, int newHeight)
        {
            bmp = (Bitmap)bitmap.Clone();
            this.height = newHeight;
            this.width = newWidth;
        }

        public Bitmap PerformManipuation()
        {
            if (width != 0 && height != 0)
            {
                var destRect = new Rectangle(0, 0, width, height);
                var destImage = new Bitmap(width, height);

                destImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(bmp, destRect, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }
                return destImage;
            }
            else
                return null;
        }

        public void SetBitmap(Bitmap bitmap)
        {
            
        }
    }
}
