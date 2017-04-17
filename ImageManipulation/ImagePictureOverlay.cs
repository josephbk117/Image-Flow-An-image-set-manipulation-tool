using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManipulation
{
    public class ImagePictureOverlay : IMageManipulation
    {
        Bitmap bmp;
        Bitmap imageToOverlay;
        Rectangle imageRect;
        int opacity;

        public ImagePictureOverlay(Bitmap bitmap, Bitmap overlayImage, int x, int y, int width, int height, int opacity)
        {
            bmp = (Bitmap)bitmap.Clone();
            imageToOverlay = overlayImage;
            imageRect = new Rectangle(x, y, width, height);
            this.opacity = (int)((255f / 100f) * Math.Max(0, Math.Min(100, opacity)));
        }
        public ImagePictureOverlay(Bitmap overlayImage, int x, int y, int width, int height, int opacity)
        {
            imageToOverlay = overlayImage;
            imageRect = new Rectangle(x, y, width, height);
            this.opacity = (int)((255f / 100f) * Math.Max(0, Math.Min(100, opacity)));
        }

        public Bitmap PerformManipuation()
        {
            Graphics gr = Graphics.FromImage(bmp);
            imageToOverlay = ImageFormatConverter.ConvertTo32Bpp(imageToOverlay);

            for (int i = 0; i < imageToOverlay.Width; i++)
            {
                for (int j = 0; j < imageToOverlay.Height; j++)
                {
                    Color nCol = imageToOverlay.GetPixel(i, j);
                    imageToOverlay.SetPixel(i, j, Color.FromArgb(opacity, nCol.R, nCol.G, nCol.B));
                }
            }

            gr.DrawImage(imageToOverlay, imageRect);

            return (Bitmap)bmp.Clone();
        }

        public void SetBitmap(Bitmap bitmap)
        {
            bmp = (Bitmap)bitmap.Clone();
        }
    }
}
