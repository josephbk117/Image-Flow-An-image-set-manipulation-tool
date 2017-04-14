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

        public ImagePictureOverlay(Bitmap bitmap,Bitmap overlayImage,int x,int y,int width,int height)
        {
            bmp = (Bitmap)bitmap.Clone();
            imageToOverlay = overlayImage;
            imageRect = new Rectangle(x, y, width, height);
        }
        public ImagePictureOverlay(Bitmap overlayImage, int x, int y, int width, int height)
        {
            imageToOverlay = overlayImage;
            imageRect = new Rectangle(x, y, width, height);
        }

        public Bitmap PerformManipuation()
        {
            Graphics gr = Graphics.FromImage(bmp);
            gr.DrawImage(imageToOverlay, imageRect);

            return (Bitmap)bmp.Clone();
        }

        public void SetBitmap(Bitmap bitmap)
        {
            bmp = (Bitmap)bitmap.Clone();
        }
    }
}
