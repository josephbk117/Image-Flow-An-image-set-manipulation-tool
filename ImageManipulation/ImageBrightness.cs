using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageManipulation
{
    public class ImageBrightness : IMageManipulation
    {
        Bitmap bmp;
        int brightness;

        public ImageBrightness(Bitmap bitmap, int brightness)
        {
            bmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
            this.brightness = brightness;
        }
        public ImageBrightness(int brightness)
        {
            this.brightness = brightness;
        }
        public void SetBitmap(Bitmap bitmap)
        {
            bmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
        }

        public Bitmap PerformManipuation()
        {
            if (brightness < -254) brightness = -254;
            if (brightness > 254) brightness = 254;

            unsafe
            {
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, bmp.PixelFormat);

                int bytesPerPixel = Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bmp.Height;
                int widthInBytes = bmp.Width * bytesPerPixel;
                byte* firstPixelPtr = (byte*)bmpData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = firstPixelPtr + (y * bmpData.Stride);

                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {

                        int currBlue = currentLine[x] + brightness;
                        int currGreen = currentLine[x + 1] + brightness;
                        int currRed = currentLine[x + 2] + brightness;

                        if (currBlue > 255) currBlue = 255;
                        if (currGreen > 255) currGreen = 255;
                        if (currRed > 255) currRed = 255;

                        if (currBlue < 0) currBlue = 0;
                        if (currGreen < 0) currGreen = 0;
                        if (currRed < 0) currRed = 0;

                        currentLine[x] = (byte)currBlue;
                        currentLine[x + 1] = (byte)currGreen;
                        currentLine[x + 2] = (byte)currRed;
                    }
                });
                bmp.UnlockBits(bmpData);
            }

            return (Bitmap)bmp.Clone();
        }
        public override string ToString()
        {
            return "Brightness : " + ((brightness > 0) ? "+" + brightness : brightness.ToString());
        }
    }
}
