using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImageManipulation
{
    public class ImageTextOverlay : IMageManipulation
    {
        Bitmap bmp;
        string text, fontName, fontStyle;
        Color colour1, colour2;
        int xPosition, yPosition;
        float fontSize;

        public ImageTextOverlay(Bitmap bitmap, string text, string fontName, string fontStyle, float fontSize,
                Color colour1, Color colour2, int xPosition, int yPosition)
        {
            bmp = (Bitmap)bitmap.Clone();
            this.text = text;
            this.fontName = fontName;
            this.fontStyle = fontStyle;
            this.colour1 = colour1;
            this.colour2 = colour2;
            this.fontSize = fontSize;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
        }

        public ImageTextOverlay(string text, string fontName, string fontStyle, float fontSize,
                Color colour1, Color colour2, int xPosition, int yPosition)
        {
            this.text = text;
            this.fontName = fontName;
            this.fontStyle = fontStyle;
            this.colour1 = colour1;
            this.colour2 = colour2;
            this.fontSize = fontSize;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
        }

        public Bitmap PerformManipuation()
        {
            Graphics gr = Graphics.FromImage(bmp);
            if (string.IsNullOrEmpty(fontName))
                fontName = "Times New Roman";
            if (fontSize.Equals(null))
                fontSize = 10.0F;
            Font font = new Font(fontName, fontSize);
            if (!string.IsNullOrEmpty(fontStyle))
            {
                FontStyle fStyle = FontStyle.Regular;
                switch (fontStyle.ToLower())
                {
                    case "bold":
                        fStyle = FontStyle.Bold;
                        break;
                    case "italic":
                        fStyle = FontStyle.Italic;
                        break;
                    case "underline":
                        fStyle = FontStyle.Underline;
                        break;
                    case "strikeout":
                        fStyle = FontStyle.Strikeout;
                        break;
                }
                font = new Font(fontName, fontSize, fStyle);
            }
            if (colour1 == null)
                colour1 = Color.Black;
            if (colour2 == null)
                colour2 = Color.White;

            int gW = (int)(text.Length * fontSize);
            gW = gW == 0 ? 10 : gW;
            LinearGradientBrush LGBrush = new LinearGradientBrush(new Rectangle(0, 0, gW, (int)fontSize), colour1, colour2, LinearGradientMode.Vertical);
            gr.DrawString(text, font, LGBrush, xPosition, yPosition);

            return (Bitmap)bmp.Clone();

        }

        public void SetBitmap(Bitmap bitmap)
        {
            bmp = (Bitmap)bitmap.Clone();
        }
    }
}
