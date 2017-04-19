using System.Drawing;

namespace ImageManipulation
{
    public class ImageCrop : IMageManipulation
    {
        Bitmap temp;
        int xPosition, yPosition, width, height;
        public ImageCrop(Bitmap bitmap, int xPosition, int yPosition, int width, int height)
        {
            temp = (Bitmap)bitmap.Clone();
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.width = width;
            this.height = height;
        }
        public ImageCrop(int xPosition, int yPosition, int width, int height)
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.width = width;
            this.height = height;
        }

        public void SetBitmap(Bitmap bitmap)
        {
            this.temp = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmap.PixelFormat);
        }

        public Bitmap PerformManipuation()
        {            
            Bitmap bmap = (Bitmap)temp.Clone();
            if (xPosition + width > temp.Width)
                width = temp.Width - xPosition;
            if (yPosition + height > temp.Height)
                height = temp.Height - yPosition;
            Rectangle rect = new Rectangle(xPosition, yPosition, width, height);
            return (Bitmap)bmap.Clone(rect, bmap.PixelFormat);
        }
        public override string ToString()
        {
            return "Crop : x = " + xPosition + ", y = " + yPosition + ", width = " + width + ", height = " + height;
        }
    }
}