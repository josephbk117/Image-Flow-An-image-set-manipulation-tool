using System.Drawing;

namespace ImageManipulation
{
    public interface IMageManipulation
    {
        Bitmap PerformManipuation();
        void SetBitmap(Bitmap bitmap);        
    }
}
