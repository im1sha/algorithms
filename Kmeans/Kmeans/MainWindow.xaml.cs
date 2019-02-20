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
using System.ComponentModel;

namespace Kmeans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            var avm = new ApplicationViewModel();
            DataContext = avm;

        }


        private BitmapSource random()
        {
            int dpi = 96;

            uint[] color = new[]
            {
                0xFFf44b42,
                0xFFf47141,
                0xFFf4ac41,
                0xFFf4d641,
                0xFFe2f441,
                0xFF79f441,
                0xFF41f4a9,
                0xFF41f4f4,
                0xFF419df4,
                0xFF414cf4,
                0xFF7c41f4,
                0xFFa941f4,
                0xFF431663,
                0xFFf141f4,
                0xFFfa00ff,
                0xFFf4419a,
                0xFFf4beb7,
                0xFFdcf4b7,
                0xFFb7e5f4,
                0xFF747474,
            };

            int totalColors = color.Length;

            int size = 1000;
            int sqrsize = 1000_000;
            Random rnd = new Random();

            uint[] pixels = new uint[sqrsize];

            for (int i = 0; i < sqrsize; i++)
            {
                pixels[i] = color[rnd.Next() % totalColors];
            }

            var bmpSource = new WriteableBitmap(size, size, dpi, dpi, PixelFormats.Pbgra32, null);

            bmpSource.WritePixels(new Int32Rect(0, 0, size, size), pixels, bmpSource.BackBufferStride, 0);

            return bmpSource;
        }
        //private async void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    txt.Text = "started";
        //    await Task.Run(() => HeavyMethod(this));
        //    txt.Text = "done";
        //}
        //internal void HeavyMethod(MainWindow gui)
        //{
        //    while (stillWorking)
        //    {
        //        window.Dispatcher.Invoke(() =>
        //        {
        //            // UI operations go inside of Invoke
        //            txt.Text += ".";
        //        });
        //        // Heavy operations go outside of Invoke
        //        System.Threading.Thread.Sleep(51);
        //    }
        //}
    }
}
