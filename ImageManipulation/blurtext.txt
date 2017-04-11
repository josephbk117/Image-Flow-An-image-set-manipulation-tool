using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManipulation
{
    public class ImageBlur : IMageManipulation
    {
        Bitmap bmp;
        int kernelSize, blurAmount;

        public ImageBlur(Bitmap bitmap, int kernelSize, int blurAmount)
        {
            bmp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
            this.kernelSize = kernelSize;
            this.blurAmount = blurAmount;
        }

        public Bitmap PerformManipuation()
        {

            Bitmap newBmp;
            unsafe
            {
                newBmp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
                //TODO : set lock mode to read only
                BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                /*-*/
                BitmapData newBmpData = newBmp.LockBits(new Rectangle(0, 0, newBmp.Width, newBmp.Height),
                    ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                int bytesPerPixel = Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
                int heightInPixels = bmp.Height;
                int widthInBytes = bmp.Width * bytesPerPixel;

                byte* firstPixelPtr = (byte*)bmpData.Scan0;
                /*-*/
                byte* firstNewPixelPtr = (byte*)newBmpData.Scan0;
                Console.WriteLine("Stride value = " + bmpData.Stride + ", " + newBmpData.Stride);
                Console.WriteLine("Width in byes = " + widthInBytes);
                Console.WriteLine("First pixel = " + firstPixelPtr[0] + "," + firstPixelPtr[1] + ", " + firstPixelPtr[2]);

                //Height in pixels same
                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = firstPixelPtr + (y * bmpData.Stride);
                    /*-*/
                    byte* newCurrentLine = firstNewPixelPtr + (y * newBmpData.Stride);

                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int prevYindex = y - 1;
                        int nextYinex = y + 1;
                        byte* prevLine;
                        byte* nextLine;
                        if (y > 0)
                            prevLine = firstPixelPtr + ((y - 1) * bmpData.Stride);
                        else
                            prevLine = firstPixelPtr + (y * bmpData.Stride);
                        if (y < heightInPixels - 1)
                            nextLine = firstPixelPtr + ((y + 1) * bmpData.Stride);
                        else
                            nextLine = firstPixelPtr + (y * bmpData.Stride);

                        int currBlue = currentLine[x];
                        int currGreen = currentLine[x + 1];
                        int currRed = currentLine[x + 2];

                        int upBlue = prevLine[x];
                        int upGreen = prevLine[x + 1];
                        int upRed = prevLine[x + 2];

                        int downBlue = nextLine[x];
                        int downGreen = nextLine[x + 1];
                        int downRed = nextLine[x + 2];

                        int leftBlue = currBlue, leftGreen = currGreen, leftRed = currRed;
                        int rightBlue = currBlue, rightGreen = currGreen, rightRed = currRed;
                        if (x - 3 >= 0)
                        {
                            leftBlue = currentLine[x - 3];
                            leftGreen = currentLine[x - 2];
                            leftRed = currentLine[x - 1];
                        }
                        if (x + 5 <= widthInBytes)
                        {
                            rightBlue = currentLine[x + 3];
                            rightGreen = currentLine[x + 4];
                            rightRed = currentLine[x + 5];
                        }

                        byte avgB = ClampValue((currBlue + upBlue + downBlue + leftBlue + rightBlue) / 5f);
                        byte avgG = ClampValue((currGreen + upGreen + downGreen + leftGreen + rightGreen) / 5f);
                        byte avgR = ClampValue((currRed + upRed + downRed + leftRed + rightRed) / 5f);

                        newCurrentLine[x] = avgB;
                        newCurrentLine[x + 1] = avgG;
                        newCurrentLine[x + 2] = avgR;
                    }
                });
                bmp.UnlockBits(bmpData);
                newBmp.UnlockBits(newBmpData);
            }

            return (Bitmap)newBmp.Clone();
        }

        public void SetBitmap(Bitmap bitmap)
        {

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
