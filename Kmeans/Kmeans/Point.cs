using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans
{
    struct Point 
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
            Cluster = -1;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public int Cluster { get; set; }

        public int GetSquareOfDistance(Point anotherPoint)
        {
            return (int) (Math.Pow(anotherPoint.X - X, 2) + Math.Pow(anotherPoint.Y - Y, 2));
        }
    }
}
