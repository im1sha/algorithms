using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans
{
    class ClusterCenter : IPoint
    {
        public const int NotSpecifiedCluster = -1;

        public int X { get; private set; }
        public int Y { get; private set; }

        public int Index { get; }

        public ClusterCenter(int x, int y, int clusterIndex)
        {
            X = x;
            Y = y;
            if (IsClusterValid(clusterIndex))
            {
                Index = clusterIndex;
            }
            else
            {
                throw new ArgumentException("invalid cluster number ");
            }
        }

        public bool ChangePosition(int x, int y)
        {
            X = x;
            Y = y;
            return true;
        }

        private bool IsClusterValid(int value)
        {
            return value > NotSpecifiedCluster;
        }

        public static bool operator ==(ClusterCenter center, ClusterCenter anotherCenter)
        {
            if (center.Index == anotherCenter.Index &&
                center.X == anotherCenter.X &&
                center.Y == anotherCenter.Y)
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(ClusterCenter center, ClusterCenter anotherCenter)
        {
            return !(center == anotherCenter);
        }

      
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
      
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
