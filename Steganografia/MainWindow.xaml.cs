using Microsoft.Win32;
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
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using System.Drawing.Imaging;

namespace Steganografia
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap inputBitmap;
        private Bitmap outputPicture;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Wybierz obraz";
            fileDialog.Filter = "Bitmapa (*.bmp) | *.bmp";

            if (fileDialog.ShowDialog() == true)
            {
                // Ustawianie źródła naszego elementu Image
                // na wybrany obrazek
                inputBitmap = new Bitmap(fileDialog.OpenFile());
                var imageWpf = new BitmapImage();
                imageWpf.BeginInit();
                imageWpf.StreamSource = fileDialog.OpenFile();
                imageWpf.EndInit();

                InputImage.Source = imageWpf;
                //InputImage.Source = new BitmapImage(new Uri(fileDialog.FileName));


                var sizeOfFile = FileDetails.GetBytesReadable(fileDialog.FileName);

                FilenameOfImageLabel.Content = "Nazwa pliku: " + fileDialog.SafeFileName;
                SizeOfImageLabel.Content = "Rozmiar: " + sizeOfFile;
                HeightOfImageLabel.Content = "Wysokość: " + InputImage.Source.Height.ToString();
                WidthOfImageLabel.Content = "Szerokość: " + InputImage.Source.Width.ToString();
            }

        }

        private void EncryptImageButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (TextToInputTextBox.Text == "")
            {
                MessageBox.Show("Nie wprowadziłeś żadnego tekstu do ukrycia", "Warning");

                return;
            }
            // Sprawdzamy czy jest jakiś obrazek ustawiony
            if (InputImage.Source != null)
            {
                // Tworzymy bitmapę, którą będziemy modyfikować
                string text = TextToInputTextBox.Text;
                outputPicture = Steganography.EmbedText(text, inputBitmap);
                OutputImage.Source = outputPicture.ConvertToWpf();

                MessageBox.Show("Your text was hidden in the image successfully!", "Done");
            }
        }

        private void DecryptImageButton_Click(object sender, RoutedEventArgs e)
        {
            string extractedText = Steganography.extractText(inputBitmap);
            TextToInputTextBox.Text = extractedText;
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Zapisz obraz";
            saveFileDialog.Filter = "Bitmapa (*.bmp) | *.bmp";

            if (saveFileDialog.ShowDialog() == true)
            {
                outputPicture.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                MessageBox.Show("Zapisano obraz pomyślnie!", "Done");
            }
            
        }
    }
}
