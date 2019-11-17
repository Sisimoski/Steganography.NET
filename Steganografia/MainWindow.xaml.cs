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
        byte[] input = null;

        public MainWindow()
        {
            InitializeComponent();
        }
        private int BytesAvailbleToSave => BitsPerPixel * BitmapSize / 8;
        private int BitmapSize => inputBitmap.Width * inputBitmap.Height;
        public int BitsPerPixel => Steganography.NumberOfRedBits + Steganography.NumberOfGreenBits + Steganography.NumberOfBlueBits;

        private void LogError(string msg, Exception exception = null)
        {
            MessageBox.Show(this, $"{msg} error: {exception}");
        }

        private void OpenImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Wybierz obraz";
            openFileDialog.DefaultExt = ".bmp";
            openFileDialog.Filter = "Pliki Bitmapy (*.bmp)|*.bmp|Pliki PNG (*.png)|*.png|Wszystkie pliki (*.*)|*.*";

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                // Ustawianie źródła naszego elementu Image
                // na wybrany obrazek
                inputBitmap = new Bitmap(openFileDialog.OpenFile());
                var imageWpf = new BitmapImage();
                imageWpf.BeginInit();
                imageWpf.StreamSource = openFileDialog.OpenFile();
                imageWpf.EndInit();

                InputImage.Source = imageWpf;
                //InputImage.Source = new BitmapImage(new Uri(fileDialog.FileName));
                input = File.ReadAllBytes(openFileDialog.FileName);


                var sizeOfFile = FileDetails.GetBytesReadable(openFileDialog.FileName);
                FilenameOfImageLabel.Content = "Nazwa pliku: " + openFileDialog.SafeFileName;
                SizeOfImageLabel.Content = "Rozmiar: " + sizeOfFile;
                HeightOfImageLabel.Content = "Wysokość: " + InputImage.Source.Height.ToString();
                WidthOfImageLabel.Content = "Szerokość: " + InputImage.Source.Width.ToString();
                AvailableBytesToSaveLabel.Content = "Ilość dostępnych bitów do zapisania: " + BytesAvailbleToSave + "B";
                
            }
            catch (Exception exception)
            {
                inputBitmap = null;
                InputImage.Source = null;
                LogError("Wczytaj", exception);
            }
        }

        private void EncryptImageButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (TextToInputTextBox.Text == "")
            {
                MessageBox.Show("Nie wprowadziłeś żadnego tekstu do ukrycia!", "Warning");
                return;
            }
            if (TextToInputTextBox.Text.Length > BytesAvailbleToSave)
            {
                MessageBox.Show("Wprowadzony jest za długi. Nie wystarczająca ilość dostępnych bajtów do zapisania.", "Error");
                return;
            }
            // Sprawdzamy czy jest jakiś obrazek ustawiony
            if (InputImage.Source != null)
            {
                // Tworzymy bitmapę, którą będziemy modyfikować
                string text = TextToInputTextBox.Text;
                outputPicture = Steganography.EmbedText(text, inputBitmap);
                OutputImage.Source = outputPicture.ConvertToWpf();

                EncodedDataLabel.Content = "Zakodowane dane: " + input.Length + "B";

                MessageBox.Show("Twój tekst został ukryty w obrazku!", "Done");
            }
        }

        private void DecryptImageButton_Click(object sender, RoutedEventArgs e)
        {
            string extractedText = Steganography.extractText(inputBitmap);
            EncodedTextTextBox.Text = extractedText;
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Zapisz obraz";
            saveFileDialog.DefaultExt = ".bmp";
            saveFileDialog.Filter = "Pliki Bitmapy (*.bmp)|*.bmp|Pliki PNG (*.png)|*.png|Wszystkie pliki (*.*)|*.*";

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }
            try
            {
                outputPicture.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                MessageBox.Show("Zapisano obraz pomyślnie!", "Done");
            } catch (Exception exception)
            {
                LogError("Zapisz", exception);
            }
            
        }
        private void RBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Steganography.NumberOfRedBits = RBits.SelectedIndex;
        }

        private void GBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Steganography.NumberOfGreenBits = GBits.SelectedIndex;
        }

        private void BBits_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Steganography.NumberOfBlueBits = BBits.SelectedIndex;
        }

        private void StretchFillComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StretchFillComboBox.SelectedIndex == 0)
            {
                InputImage.Stretch = Stretch.Uniform;
                OutputImage.Stretch = Stretch.Uniform;
            } 
            if (StretchFillComboBox.SelectedIndex == 1)
            {
                InputImage.Stretch = Stretch.Fill;
                OutputImage.Stretch = Stretch.Fill;
            }
        }
    }
}
