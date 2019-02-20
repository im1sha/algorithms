using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans
{
    struct StaticPoint
    {
        public readonly int X;
        public readonly int Y;

        public StaticPoint(int x, int y) 
        {
            X = x;
            Y = y;
        }
     
        public int GetSquareOfDistanceTo(StaticPoint anotherPoint)
        {
            return (int)(Math.Pow(anotherPoint.X - X, 2.0) + Math.Pow(anotherPoint.Y - Y, 2.0));
        }

        public static bool operator ==(StaticPoint point1, StaticPoint point2)
        {
            return (point1.X == point2.X) && (point1.Y == point2.Y);
        }

        public static bool operator !=(StaticPoint point1, StaticPoint point2)
        {
            return !(point1 == point2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}
