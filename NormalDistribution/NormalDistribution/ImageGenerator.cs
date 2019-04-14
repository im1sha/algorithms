using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NormalDistribution
{
    class ImageGenerator
    {
        static Color GetGrayColor() => Color.FromRgb(100, 100, 100); 
        static Color GetRedColor() => Color.FromRgb(255, 0, 0);
        static Color GetBlueColor() => Color.FromRgb(0, 0, 255); 

        public static BitmapSource GetImageByData(
            int[] chart1,
            int[] chart2,
            int intersectionLine, 
            int size, 
            int dpi = 96)
        {
            var bitmap = new WriteableBitmap(size, size, dpi, dpi, PixelFormats.Pbgra32, null);

            bitmap.DrawPolyline(chart1, GetBlueColor());
            bitmap.DrawPolyline(chart2, GetRedColor());
            bitmap.DrawLine(intersectionLine, 0, intersectionLine, size - 1, GetGrayColor());

            return bitmap;
        }
    }
}
