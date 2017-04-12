﻿using System;
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

namespace ImageSetManipulationToolWpf
{

    public partial class MainWindow : Window
    {
        string[] files;
        List<IMageManipulation> manipulations;
        public MainWindow()
        {
            InitializeComponent();
            manipulations = new List<IMageManipulation>();
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
            AddToOperationStack("Brightness : " + brightnessValueText.Text);
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

            AddToOperationStack("Contrast : " + contrastValueText.Text);
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
            AddToOperationStack("Desaturation : " + desaturationValueText.Text + "%");
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
            AddToOperationStack("Crop : " + cropXValue.Text + "," + cropYValue.Text + "," + cropWidthValue.Text + "," + cropHeightValue.Text);
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
            AddToOperationStack("Resize : " + resizeWidth.Text + "," + resizeHeight.Text);
        }
        private void AddToOperationStack(string content)
        {
            operationStack.Children.Add(new System.Windows.Controls.Label() { Content = content, HorizontalAlignment = System.Windows.HorizontalAlignment.Center, Foreground = System.Windows.Media.Brushes.White,
                FontSize=16});
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
            Console.WriteLine("LOLz");
            ((System.Windows.Controls.TextBox)(sender)).Text = "";
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
            AddToOperationStack("Blur : " + "Small ," + blurValue.Text);
        }
    }
}
