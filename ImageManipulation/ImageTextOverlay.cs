using System.Drawing;

namespace ImageManipulation
{
    public class ImageTextOverlay : IMageManipulation
    {
        Bitmap bmp;
        string text, fontName, fontStyle;
        Color colour;
        int xPosition, yPosition;
        float fontSize;

        public ImageTextOverlay(Bitmap bitmap, string text, string fontName, string fontStyle, float fontSize,
                Color colour, int xPosition, int yPosition)
        {
            bmp = (Bitmap)bitmap.Clone();
            this.text = text;
            this.fontName = fontName;
            this.fontStyle = fontStyle;
            this.colour = colour;
            this.fontSize = fontSize;
            this.xPosition = xPosition;
            this.yPosition = yPosition;
        }

        public ImageTextOverlay(string text, string fontName, string fontStyle, float fontSize,
                Color colour, int xPosition, int yPosition)
        {
            this.text = text;
            this.fontName = fontName;
            this.fontStyle = fontStyle;
            this.colour = colour;
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
            if (colour == null)
                colour = Color.Black;


            int gW = (int)(text.Length * fontSize);
            gW = gW == 0 ? 10 : gW;
            SolidBrush LGBrush = new SolidBrush(colour);
            gr.DrawString(text, font, LGBrush, xPosition, yPosition);

            return (Bitmap)bmp.Clone();

        }

        public void SetBitmap(Bitmap bitmap)
        {
            bmp = (Bitmap)bitmap.Clone();
        }
        public override string ToString()
        {
            return "Text Overlay : " + "text = " + text + ", font = " + fontName + ", font size = " + fontSize + ", colour = " + colour.ToString() + ", x = " + xPosition + ", y = " + yPosition;
        }
    }
}
