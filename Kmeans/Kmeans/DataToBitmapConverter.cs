﻿using System;
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
        public static BitmapSource Convert(/*uint[] pixels*/)
        {
            int dpi = 96;

            uint[] color = new[] {
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

            var pixels = new uint[sqrsize];

            for (int i = 0; i < sqrsize; i++)
            {
                pixels[i] = color[rnd.Next() % totalColors];
            }

            var bmpSource = new WriteableBitmap(size, size, dpi, dpi, PixelFormats.Pbgra32, null);

            bmpSource.WritePixels(new Int32Rect(0, 0, size, size), pixels, bmpSource.BackBufferStride, 0);

            return bmpSource;
        }

    }
}
