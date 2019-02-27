using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Maximin
{
    static class DataToBitmapConverter
    {
        public const uint WHITE_COLOR = 0xFFFFFFFF;
        public const uint BLACK_COLOR = 0xFF000000;

        public static BitmapSource ClustersToBitmap((StaticPoint Сenter, StaticPoint[] StaticPoints)[] clusters,
            int sizeX, int sizeY, uint[] colors)
        {
            uint [] pixels = GenerateColorsArray(clusters, sizeX, sizeY, colors);

            return ArrayToBitmap(pixels, sizeX, sizeY);
        }


        /// <summary>
        /// Converts sequence of colors represented in format #AARRGGBB to bitmap
        /// </summary>
        /// <param name="pixels">Array containing color for each the pixel</param>
        /// <param name="sizeX">Horizontal image dimension</param>
        /// <param name="sizeY">Vertical image dimension</param>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        /// <returns>Image represented as BitmapSource</returns>
        private static BitmapSource ArrayToBitmap(uint[] pixels, int sizeX, int sizeY, 
            int dpiX = 96,int dpiY = 96)
        {
            if (sizeX * sizeY != pixels.Length)
            {
                throw new ArgumentException("Wrong number of pixels.");
            }
            var bmpSource = new WriteableBitmap(sizeX, sizeY, dpiX, dpiY, PixelFormats.Pbgra32, null);
            bmpSource.WritePixels(new Int32Rect(0, 0, sizeX, sizeY), pixels, bmpSource.BackBufferStride, 0);

            return bmpSource;
        }

        /// <summary>
        /// Generates representation of clusters as numeric array
        /// </summary>
        /// <param name="clusters"></param>      
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="colors">Colors in format #AARRGGBB</param>
        /// <returns></returns>
        private static uint[] GenerateColorsArray(
            (StaticPoint Сenter, StaticPoint[] StaticPoints)[] clusters,
            int sizeX, int sizeY, uint[] colors)
        {
            const int CENTRAL_RADIUS = 3;
            const int USUAL_RADIUS = 1;

            if (clusters.Length != colors.Length)
            {
                throw new ArgumentException("Arguments should have the same length.");
            }

            int totalPoints = sizeX * sizeY;

            uint[] pixels = Enumerable.Repeat(WHITE_COLOR, totalPoints).ToArray();

            // draw other points
            for (int i = 0; i < clusters.Length; i++)
            {
                for (int j = 0; j < clusters[i].StaticPoints.Length; j++)
                {
                    DrawPoint(pixels, clusters[i].StaticPoints[j].X, clusters[i].StaticPoints[j].Y, sizeX, sizeY, USUAL_RADIUS, colors[i]);
                }
            }
       
            // draw central points
            for (int i = 0; i < clusters.Length; i++)
            {         
                DrawPoint(pixels, clusters[i].Сenter.X, clusters[i].Сenter.Y, sizeX, sizeY, CENTRAL_RADIUS, BLACK_COLOR);
            }

            return pixels;
        }

        private static uint[] DrawPoint(uint[] pixels, int x, int y, int sizeX, int sizeY, int radius, uint color)
        {
            for (int j = -radius; j <= radius; j++)
            {
                for (int k = -radius; k <= radius; k++)
                {
                    if ((x + j < sizeX) && (x + j > 0) &&
                        (y + k < sizeY) && (y + k > 0))
                    {
                        pixels[x + j + (y + k) * sizeX] = color;
                    }
                }
            }

            return pixels;
        }
    }
}
