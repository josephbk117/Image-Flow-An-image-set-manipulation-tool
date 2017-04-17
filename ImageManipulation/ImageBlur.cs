using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageManipulation
{
    public class ImageBlur : IMageManipulation
    {
        Bitmap bmp;
        int kernelSize, blurAmount;       

        public enum KernelSize
        {
            Small,
            Medium,
            Large
        }

        public ImageBlur(Bitmap bitmap, KernelSize kernelSize, int blurAmount)
        {
            bmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
            switch(kernelSize)
            {
                case KernelSize.Small:
                    this.kernelSize = 5;break;
                case KernelSize.Medium:
                    this.kernelSize = 7;break;
                case KernelSize.Large:
                    this.kernelSize = 11;break;
            }            
            this.blurAmount = blurAmount;
        }
        public ImageBlur(KernelSize kernelSize, int blurAmount)
        {            
            switch (kernelSize)
            {
                case KernelSize.Small:
                    this.kernelSize = 5; break;
                case KernelSize.Medium:
                    this.kernelSize = 7; break;
                case KernelSize.Large:
                    this.kernelSize = 11; break;
            }
            this.blurAmount = blurAmount;
        }

        public Bitmap PerformManipuation()
        {           
            Bitmap newBmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);

            for (int i=0;i<blurAmount;i++)
            {
                newBmp = Blur(newBmp);
            }

            return (Bitmap)newBmp.Clone();
        }

        private Bitmap Blur(Bitmap bitmap)
        {
            Bitmap newBmp;
            unsafe
            {
                newBmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);

                BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                BitmapData newBmpData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int bytesPerPixel = Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bmp.Height;
                int widthInBytes = bmp.Width * bytesPerPixel;

                byte* firstPixelPtr = (byte*)bmpData.Scan0;
                byte* firstNewPixelPtr = (byte*)newBmpData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = firstPixelPtr + (y * bmpData.Stride);
                    byte* newCurrentLine = firstNewPixelPtr + (y * newBmpData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int avgBl = 0;
                        int avgGl = 0;
                        int avgRl = 0;
                        for (int line = y - kernelSize / 2; line <= y + kernelSize / 2; line++)
                        {
                            if (line > 0 && line < heightInPixels)
                            {
                                byte* linePtr = firstPixelPtr + (line * bmpData.Stride);

                                if (x - 6 > 0 && x + 9 < widthInBytes)
                                {
                                    for (int val = x - (2 * 3); val < x + (3 * 3); val += 3)
                                    {
                                        avgBl += linePtr[val];
                                        avgGl += linePtr[val + 1];
                                        avgRl += linePtr[val + 2];
                                    }
                                }
                            }
                        }
                        newCurrentLine[x] = (byte)(avgBl / (kernelSize * kernelSize));
                        newCurrentLine[x + 1] = (byte)(avgGl / (kernelSize * kernelSize));
                        newCurrentLine[x + 2] = (byte)(avgRl / (kernelSize * kernelSize));
                    }
                });
                bitmap.UnlockBits(bmpData);
                newBmp.UnlockBits(newBmpData);
            }

            return (Bitmap)newBmp.Clone();
        }

        public void SetBitmap(Bitmap bitmap)
        {
            bmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
        }

        private byte ClampValue(float value)
        {
            if (value > 255)
                return 255;
            else if (value < 0)
                return 0;
            else return (byte)value;
        }
    }
}
