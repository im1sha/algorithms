using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kmeans
{
    static class DataToBitmapConverter
    {
        //uint[] color = new[] {
        //        0xFFf44b42,
        //        0xFFf47141,
        //        0xFFf4ac41,
        //        0xFFf4d641,
        //        0xFFe2f441,
        //        0xFF79f441,
        //        0xFF41f4a9,
        //        0xFF41f4f4,
        //        0xFF419df4,
        //        0xFF414cf4,
        //        0xFF7c41f4,
        //        0xFFa941f4,
        //        0xFF431663,
        //        0xFFf141f4,
        //        0xFFfa00ff,
        //        0xFFf4419a,
        //        0xFFf4beb7,
        //        0xFFdcf4b7,
        //        0xFFb7e5f4,
        //        0xFF747474,
        //    };

        /// <summary>
        /// Converts sequence of colors represented in format #AARRGGBB to bitmap
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="pixels">Array containing color for each pixel</param>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        /// <returns>Image represented as BitmapSource</returns>
        public static BitmapSource Convert(int sizeX, int sizeY, uint[] pixels, 
            int dpiX = 96,int dpiY = 96)
        {        
            var bmpSource = new WriteableBitmap(sizeX, sizeY, dpiX, dpiY, PixelFormats.Pbgra32, null);
            bmpSource.WritePixels(new Int32Rect(0, 0, sizeX, sizeY), pixels, bmpSource.BackBufferStride, 0);
            return bmpSource;
        }




    }
}
