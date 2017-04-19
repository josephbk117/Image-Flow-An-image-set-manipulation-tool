using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using ImageManipulation;
using System.Drawing.Text;

namespace ImageSetManipulationToolWpf
{

    public partial class MainWindow : Window
    {
        string[] files;
        List<IMageManipulation> manipulations;

        System.Drawing.Color textColourFromChoice;

        private static int selectedStackOperation = -1;
        List<string> initialText;
        List<System.Windows.Controls.TextBox> textBoxes;
        public MainWindow()
        {
            InitializeComponent();
            manipulations = new List<IMageManipulation>();
            SetUpFonts();
            textColourFromChoice = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            textBoxes = new List<System.Windows.Controls.TextBox>();
            initialText = new List<string>();
            for(int i=0;i<operationControls.Children.Count;i++)
            {               
                DockPanel dPan = operationControls.Children[i] as DockPanel;
                for(int j=0;j<dPan.Children.Count;j++)
                {                    
                    if(dPan.Children[j] is System.Windows.Controls.TextBox)
                    {
                        textBoxes.Add(dPan.Children[j] as System.Windows.Controls.TextBox);
                        initialText.Add(((System.Windows.Controls.TextBox)(dPan.Children[j])).Text);
                        ((System.Windows.Controls.TextBox)(dPan.Children[j])).LostFocus += TextBox_OnFocusLost;
                    }
                }
            }            
        }

        private void SetUpFonts()
        {

            System.Drawing.FontFamily[] fontFams = new InstalledFontCollection().Families;
            for (int i = 0; i < fontFams.Length; i++)
            {
                fontFamilyComboBox.Items.Add(fontFams[i].Name);
            }

        }

        private void openFolderButton_OnLBU(object sender, MouseButtonEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (Equals(openFolderButton1, sender))
            {
                fbd.ShowNewFolderButton = false;
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    files = Directory.GetFiles(fbd.SelectedPath);
                    if (files.Length == 0)
                    {
                        DialogShow("No Input", "There are no image files in folder");
                        return;
                    }
                    folderPathTextBox.Text = fbd.SelectedPath;
                }
            }
            else
            {
                fbd.ShowNewFolderButton = true;
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    outputFolderPathTextBox.Text = fbd.SelectedPath;
                }
            }
        }

        private void generateImagesButton_OnLBU(object sender, MouseButtonEventArgs e)
        {
            if (operationStack.Children.Count < 2)
            {
                DialogShow("No Operations", "There are no operations in the stack");
                return;
            }
            Bitmap[] bitmaps;
            try
            {
                bitmaps = new Bitmap[files.Length];
            }
            catch (NullReferenceException)
            {
                DialogShow("No Input", "Provide input image files");
                return;
            }
            for (int i = 0; i < bitmaps.Length; i++)
            {
                bitmaps[i] = new Bitmap(files[i]);

                foreach (IMageManipulation man in manipulations)
                {
                    man.SetBitmap(bitmaps[i]);
                    PerformManiplation(man, ref bitmaps[i]);
                }
                bitmaps[i].Save(outputFolderPathTextBox.Text + "/outImage" + i + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            }
        }
        private void PerformManiplation(IMageManipulation manipulation, ref Bitmap bmp)
        {
            bmp = manipulation.PerformManipuation();
        }

        private void AddBrightness(object sender, RoutedEventArgs e)
        {
            try
            {
                manipulations.Add(new ImageBrightness(int.Parse(brightnessValueText.Text)));
            }
            catch (FormatException)
            {
                DialogShow("Wrong Input", "Input correct brightness value, Range(-255 to 255)");
                return;
            }
            AddToOperationStack(manipulations[manipulations.Count - 1].ToString());
        }

        private void AddContrast(object sender, RoutedEventArgs e)
        {
            try
            {
                manipulations.Add(new ImageContrast(double.Parse(contrastValueText.Text)));
            }
            catch (FormatException)
            {
                DialogShow("Wrong Input", "Input correct contast value, Range(0.0 - 100.0)");
                return;
            }

            AddToOperationStack(manipulations[manipulations.Count - 1].ToString());
        }

        private void AddDesaturation(object sender, RoutedEventArgs e)
        {
            try
            {
                manipulations.Add(new ImageDesaturation(int.Parse(desaturationValueText.Text)));
            }
            catch (FormatException)
            {
                DialogShow("Wrong Input", "Input desaturation percentage , Range(0 - 100)");
                return;
            }
            AddToOperationStack(manipulations[manipulations.Count - 1].ToString());
        }

        private void AddCrop(object sender, RoutedEventArgs e)
        {
            try
            {
                manipulations.Add(new ImageCrop(int.Parse(cropXValue.Text), int.Parse(cropYValue.Text), int.Parse(cropWidthValue.Text), int.Parse(cropHeightValue.Text)));
            }
            catch (FormatException)
            {
                DialogShow("Wrong Input", "Check your crop parameters");
                return;
            }
            AddToOperationStack(manipulations[manipulations.Count - 1].ToString());
        }
        private void AddResize(object sender, RoutedEventArgs e)
        {
            try
            {
                manipulations.Add(new ImageResize(int.Parse(resizeWidth.Text), int.Parse(resizeHeight.Text)));
            }
            catch (FormatException)
            {
                DialogShow("Wrong Input", "Check your resize parameters");
                return;
            }
            AddToOperationStack(manipulations[manipulations.Count - 1].ToString());
        }
        private void AddTextOverlay(object sender, RoutedEventArgs e)
        {
            try
            {
                manipulations.Add(new ImageTextOverlay(textOverlayText.Text, fontFamilyComboBox.SelectedItem.ToString(), "normal", float.Parse(fontSize.Text), textColourFromChoice,
                    int.Parse(xOffset.Text), int.Parse(yOffset.Text)));
            }
            catch (FormatException)
            {
                DialogShow("Wrong Input", "Check your text attribute values");
                return;
            }
            AddToOperationStack(manipulations[manipulations.Count - 1].ToString());
        }
        private void AddBlur(object sender, RoutedEventArgs e)
        {
            try
            {
                ImageBlur.KernelSize kernelSize;
                switch ((string)kernelComboBox.SelectedItem.ToString())
                {
                    case "Small": kernelSize = ImageBlur.KernelSize.Small; break;
                    case "Medium": kernelSize = ImageBlur.KernelSize.Medium; break;
                    case "Large": kernelSize = ImageBlur.KernelSize.Large; break;
                    default: kernelSize = ImageBlur.KernelSize.Small; break;
                }

                manipulations.Add(new ImageBlur(kernelSize, int.Parse(blurValue.Text)));
            }
            catch (FormatException)
            {
                DialogShow("Wrong Input", "Check your blur value");
                return;
            }
            AddToOperationStack(manipulations[manipulations.Count - 1].ToString());
        }
        private void AddImageOverlay(object sender, RoutedEventArgs e)
        {
            try
            {
                manipulations.Add(new ImagePictureOverlay(new Bitmap(overlayImageTextBox.Text), int.Parse(xImageOffset.Text),
                    int.Parse(yImageOffset.Text), int.Parse(imageWidth.Text), int.Parse(imageHeight.Text), int.Parse(imageOpacity.Text)));
            }
            catch (FormatException)
            {
                DialogShow("Wrong Input", "Check your co-ordinate values");
                return;
            }
            AddToOperationStack(((ImagePictureOverlay)(manipulations[manipulations.Count - 1])).ToString(overlayImageTextBox.Text));
        }

        private void AddToOperationStack(string content)
        {
            System.Windows.Controls.Label lbl = new System.Windows.Controls.Label()
            {
                Content = content,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 16
            };
            if (operationStack.Children.Count % 2 != 0)
                lbl.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 205, 70));
            lbl.MouseLeftButtonDown += OperationStack_ItemSelect;

            operationStack.Children.Add(lbl);
        }

        private void OperationStack_ItemSelect(object sender, MouseButtonEventArgs e)
        {
            movUp = 0;
            selectedStackOperation = operationStack.Children.IndexOf((UIElement)sender);
            //Deselect others
            for (int i = 0; i < operationStack.Children.Count; i++)
            {
                if (i != selectedStackOperation)
                {
                    ((System.Windows.Controls.Label)operationStack.Children[i]).Foreground = System.Windows.Media.Brushes.White;
                }
            }
            ((System.Windows.Controls.Label)sender).Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void RemoveFromOperationStack()
        {
            if (operationStack.Children.Count >= 1)
            {
                if (selectedStackOperation == -1)
                {
                    operationStack.Children.RemoveAt(operationStack.Children.Count - 1);
                    manipulations.RemoveAt(manipulations.Count - 1);
                }
                else
                {
                    operationStack.Children.RemoveAt(selectedStackOperation);
                    manipulations.RemoveAt(selectedStackOperation);
                }
                selectedStackOperation = -1;
            }
        }

        private void dialogRectangleOk_LBU(object sender, MouseButtonEventArgs e)
        {
            dialogGrid.Visibility = Visibility.Hidden;
        }

        private void DialogShow(string message, string content)
        {
            dialogGrid.IsHitTestVisible = true;
            dialogGrid.Visibility = Visibility.Visible;
            dialogMessageTitle.Content = message;
            dialogMessageContent.Content = content;
        }

        private void TextboxClearOnLMU(object sender, MouseButtonEventArgs e)
        {
            ((System.Windows.Controls.TextBox)(sender)).Text = "";
        }
        int movUp = 0;
        private void StackOperationButtons_OnClick(object sender, MouseButtonEventArgs e)
        {
            if (sender.Equals(removeStackOperator))
            {
                RemoveFromOperationStack();
            }
            else if (sender.Equals(moveUpStackOperator))
            {
                if (operationStack.Children.Count >= 2)
                {
                    int index = selectedStackOperation - movUp;
                    if (index > 0)
                    {
                        ReplaceStackObjects(index - 1, index);
                        movUp++;
                    }
                }
            }
        }
        private void ReplaceStackObjects(int index1, int index2)
        {
            System.Windows.Controls.Label lbl1 = (System.Windows.Controls.Label)operationStack.Children[index1];
            System.Windows.Controls.Label lbl2 = (System.Windows.Controls.Label)operationStack.Children[index2];

            operationStack.Children.Remove(lbl1);
            operationStack.Children.Remove(lbl2);

            operationStack.Children.Insert(index1, lbl2);
            operationStack.Children.Insert(index2, lbl1);

            IMageManipulation op1 = manipulations[index1];
            IMageManipulation op2 = manipulations[index2];

            manipulations.Remove(op1);
            manipulations.Remove(op2);

            manipulations.Insert(index1, op2);
            manipulations.Insert(index2, op1);

        }
        private void ImageOverlayButon_OnClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                overlayImageTextBox.Text = ofd.FileName;
            }
        }

        private void ColourSelection_OnClick(object sender, MouseButtonEventArgs e)
        {
            colourDialog.IsHitTestVisible = true;
            colourDialog.Visibility = Visibility.Visible;
        }

        private void ColourValue_OnChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            colourDisplay.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)transColour.Value, (byte)redColour.Value, (byte)greenColour.Value, (byte)blueColour.Value));
        }

        private void colourDialogOkBtn_OnClick(object sender, MouseButtonEventArgs e)
        {
            colourDialog.IsHitTestVisible = false;
            colourDialog.Visibility = Visibility.Hidden;
            textColour.Fill = colourDisplay.Fill;
            textColourFromChoice = System.Drawing.Color.FromArgb((byte)transColour.Value, (byte)redColour.Value, (byte)greenColour.Value, (byte)blueColour.Value);
        }

        private void TextBox_OnFocusLost(object sender, RoutedEventArgs e)
        {
            if ((sender as System.Windows.Controls.TextBox).Text == "")
            {
                int index = textBoxes.IndexOf(sender as System.Windows.Controls.TextBox);
                (sender as System.Windows.Controls.TextBox).Text = initialText[index];
            }
        }
    }
}
