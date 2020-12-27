using System;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CourseWork20.Controls
{
    /// <summary>
    /// Логика взаимодействия для ListItemFM.xaml
    /// </summary>
    public partial class ListItemFM : UserControl
    {
        public string Path { get; set; }
        public Bitmap ImagePath { get; set; }
        public ListItemFM()
        {
            InitializeComponent();
        }
        public ListItemFM(string pathFile, Bitmap imagePath)
        {
            InitializeComponent();
            Path = pathFile;
            ImagePath = imagePath;

            item_image.Source = Convert(imagePath);
            item_text.Text = Path;
        }
        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
