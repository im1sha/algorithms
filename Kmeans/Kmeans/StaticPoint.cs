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

        private int cluster;
        public int Cluster
        {
            get { return cluster; }
            set
            {
                if (IsClusterValid(value))
                {
                    cluster = value;
                }
                else
                {
                    throw new ArgumentException("invalid cluster value");
                }
            }
        }

        public StaticPoint(int x, int y, int cluster)
        {
            X = x;
            Y = y;
            if (IsClusterValid(cluster))
            {
                Cluster = cluster;
            }
            else
            {
                throw new ArgumentException("invalid cluster");
            }
        }

        public int GetSquareOfDistance(IPoint anotherPoint)
        {
            return (int)(Math.Pow(anotherPoint.X - X, 2.0) + Math.Pow(anotherPoint.Y - Y, 2.0));
        }

        private bool IsClusterValid(int value)
        {
            return value >= NotSpecifiedCluster;
        }
    }
}
