using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageManipulation
{
    public class ImageDesaturation : IMageManipulation
    {
        Bitmap bmp;
        int desaturationPercentage;
        public ImageDesaturation(Bitmap bitmap, int desaturationPercentage)
        {
            bmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
            this.desaturationPercentage = desaturationPercentage;
        }
        public ImageDesaturation(int desaturationPercentage)
        {            
            this.desaturationPercentage = desaturationPercentage;
        }

        public void SetBitmap(Bitmap bitmap)
        {
            bmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
        }


        public Bitmap PerformManipuation()
        {
            if (desaturationPercentage < 0) desaturationPercentage = 0;
            if (desaturationPercentage > 100) desaturationPercentage = 100;

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

                        int currBlue = currentLine[x];
                        int currGreen = currentLine[x + 1];
                        int currRed = currentLine[x + 2];

                        int avgColor = (currBlue + currGreen + currRed) / 3;
                        float actualPercentage = desaturationPercentage / 100f;

                        currBlue = (int)(((avgColor - currBlue) * actualPercentage) + currBlue);
                        currGreen = (int)(((avgColor - currGreen) * actualPercentage) + currGreen);
                        currRed = (int)(((avgColor - currRed) * actualPercentage) + currRed);

                        currentLine[x] = (byte)currBlue;
                        currentLine[x + 1] = (byte)currGreen;
                        currentLine[x + 2] = (byte)currRed;
                    }
                });
                bmp.UnlockBits(bmpData);
            }

            return (Bitmap)bmp.Clone();
        }
    }
}
