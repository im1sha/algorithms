using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans
{
    class StaticPoint : IPoint
    {
        public const int NotSpecifiedCluster = -1;

        public int X { get; }
        public int Y { get; }
        public int ClusterIndex { get; }

        public StaticPoint(int x, int y, int cluster) 
        {
            X = x;
            Y = y;
            if (!IsClusterValid(cluster))
            {
                throw new ArgumentException("Argument cluster is invalid.");
            }
            ClusterIndex = cluster;
        }
     
        private bool IsClusterValid(int value)
        {
            return value >= NotSpecifiedCluster;
        }

        public int GetDistanceTo(IPoint anotherPoint)
        {
            return (int)Math.Pow(Math.Pow(anotherPoint.X - X, 2.0) + Math.Pow(anotherPoint.Y - Y, 2.0), 0.5);
        }

        public static StaticPoint Copy(StaticPoint source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }
            return new StaticPoint(source.X, source.Y, source.ClusterIndex);
        }
    }
}
