using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageManipulation
{
    public class ImageRotation : IMageManipulation
    {
        Bitmap bmp;        
        float angle;
        public ImageRotation(Bitmap bitmap, float angleToRotate)
        {
            bmp = (Bitmap)bitmap.Clone();
            this.angle = angleToRotate;
        }

        public ImageRotation(float angleToRotate)
        {            
            this.angle = angleToRotate;
        }

        public Bitmap PerformManipuation()
        {

            float calcSinVal = (float)Math.Abs(Math.Sin(angle));
            float calcCosVal = (float)Math.Abs(Math.Cos(angle));
            int newImgWidth = (int)Math.Round((calcSinVal * bmp.Height + calcCosVal * bmp.Width));
            int newImgHeight = (int)Math.Round((calcSinVal * bmp.Width + calcCosVal * bmp.Height));

            Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height);

            //This is source to destination - bad
            //iterating through og bmp then putting it on product

            //co ordinate space
            //From -widht/2 to width/2 and -height/2 to height/2
            //(0 , 0) -> (width/2,height/2) = 0 + width/2,0+height/2 , Assume widht = 100 and height = 100
            //(-50,-50) = 0,0 -> top left

            //If image of different size , modification needed.

            /*for (int i = -bmp.Height / 2; i < bmp.Height / 2; i++)
            {
                for (int j = -bmp.Width / 2; j < bmp.Width / 2; j++)
                {
                    int xNewPos = (int)Math.Ceiling((((double)i * cosVal) - ((double)j * sinVal)));
                    int yNewPos = (int)Math.Ceiling((((double)j * cosVal) + ((double)i * sinVal)));

                    Console.WriteLine("x new = " + xNewPos + " :: y new pos = " + yNewPos);

                    if (xNewPos >= -widthVal && xNewPos < widthVal && yNewPos >= -heightVal && yNewPos < heightVal)
                    {

                        newBmp.SetPixel(xNewPos + widthVal, yNewPos + heightVal, bmp.GetPixel(i + (int)Math.Round(bmp.Width / 2f), j + (int)Math.Round(bmp.Height / 2f)));
                        Console.WriteLine("Set pixel cords = " + (xNewPos + widthVal) + " , " + (yNewPos + heightVal));

                    }
                }
            }*/

            int iCentreX = bmp.Width / 2;
            int iCentreY = bmp.Height / 2;

            for (int i = 0; i < newBmp.Height; ++i)
            {
                for (int j = 0; j < newBmp.Width; ++j)
                {
                    // convert raster to Cartesian
                    int x = j - iCentreX;
                    int y = iCentreY - i;

                    // convert Cartesian to polar
                    float fDistance = (float)Math.Sqrt(x * x + y * y);
                    float fPolarAngle = 0.0f;
                    if (x == 0)
                    {
                        if (y == 0)
                        {
                            // centre of image, no rotation needed
                            newBmp.SetPixel(j, i, bmp.GetPixel(j, i));
                            continue;
                        }
                        else if (y < 0)
                        {
                            fPolarAngle = 1.5f * (float)Math.PI;
                        }
                        else
                        {
                            fPolarAngle = 0.5f * (float)Math.PI;
                        }
                    }
                    else
                    {
                        fPolarAngle = (float)Math.Atan2((double)y, (double)x);
                    }

                    // the crucial rotation part
                    // "reverse" rotate, so minus instead of plus
                    fPolarAngle -= angle;

                    // convert polar to Cartesian
                    x = (int)(Math.Round(fDistance * Math.Cos(fPolarAngle)));
                    y = (int)(Math.Round(fDistance * Math.Sin(fPolarAngle)));

                    // convert Cartesian to raster
                    x = x + iCentreX;
                    y = iCentreY - y;

                    // check bounds
                    if (x < 0 || x >= bmp.Width || y < 0 || y >= bmp.Height) continue;

                    newBmp.SetPixel(j, i, bmp.GetPixel(x, y));
                }
            }


            return (Bitmap)newBmp.Clone();
        }

        public void SetBitmap(Bitmap bitmap)
        {
            bmp = (Bitmap)bitmap.Clone();
        }
        public override string ToString()
        {
            return "Rotate : " + angle;
        }
    }
}
