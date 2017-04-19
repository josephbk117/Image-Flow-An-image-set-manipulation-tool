using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageManipulation
{
    public class ImageContrast : IMageManipulation
    {
        Bitmap bmp;
        double contrast;

        public ImageContrast(Bitmap bitmap, double contrast)
        {
            this.bmp = bitmap.Clone(new Rectangle(0,0,bitmap.Width,bitmap.Height),bitmap.PixelFormat);
            this.contrast = contrast;
        }
        public ImageContrast(double contrast)
        {            
            this.contrast = contrast;
        }

        public void SetBitmap(Bitmap bitmap)
        {
            this.bmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
        }

        public Bitmap PerformManipuation()
        {
            if (contrast < -100) contrast = -100;
            if (contrast > 100) contrast = 100;
            contrast = (100.0 + contrast) / 100.0;
            contrast *= contrast;
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

                        double pR = currRed / 255.0;
                        pR -= 0.5;
                        pR *= contrast;
                        pR += 0.5;
                        pR *= 255;
                        if (pR < 0) pR = 0;
                        if (pR > 255) pR = 255;

                        double pG = currGreen / 255.0;
                        pG -= 0.5;
                        pG *= contrast;
                        pG += 0.5;
                        pG *= 255;
                        if (pG < 0) pG = 0;
                        if (pG > 255) pG = 255;

                        double pB = currBlue / 255.0;
                        pB -= 0.5;
                        pB *= contrast;
                        pB += 0.5;
                        pB *= 255;
                        if (pB < 0) pB = 0;
                        if (pB > 255) pB = 255;

                        currentLine[x] = (byte)pB;
                        currentLine[x + 1] = (byte)pG;
                        currentLine[x + 2] = (byte)pR;
                    }
                });
                bmp.UnlockBits(bmpData);                
            }

            return (Bitmap)bmp.Clone();
        }
        public override string ToString()
        {
            return "Contrast : " + contrast;
        }
    }
}
