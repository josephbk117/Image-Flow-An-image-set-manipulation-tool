using System;
using System.Drawing;
using System.Drawing.Imaging;
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
            imageToOverlay = ImageFormatConverter.ConvertTo32Bpp(imageToOverlay, ImageFormat.WithAlpha);

            unsafe
            {
                BitmapData bmpData = imageToOverlay.LockBits(new Rectangle(0, 0, imageToOverlay.Width, imageToOverlay.Height), ImageLockMode.ReadWrite, imageToOverlay.PixelFormat);

                int bytesPerPixel = 4;
                int heightInPixels = imageToOverlay.Height;
                int widthInBytes = imageToOverlay.Width * bytesPerPixel;
                byte* firstPixelPtr = (byte*)bmpData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = firstPixelPtr + (y * bmpData.Stride);

                    for (int x = 0; x < widthInBytes; x = x + 4)
                    {
                        currentLine[x + 3] = (byte)opacity;
                    }
                });
                imageToOverlay.UnlockBits(bmpData);
            }

            gr.DrawImage(imageToOverlay, imageRect);
            return (Bitmap)bmp.Clone();
        }
        public void SetBitmap(Bitmap bitmap)
        {
            bmp = (Bitmap)bitmap.Clone();
        }
        public string ToString(string imageName)
        {
            return "Picture Overlay : " + imageName + ", x = " + imageRect.X + ", y = " + imageRect.Y + ", width = " + imageRect.Width + ", height = " + imageRect.Height;
        }
    }
}
