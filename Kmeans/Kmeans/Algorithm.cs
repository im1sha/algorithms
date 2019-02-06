using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans
{
    class Algorithm
    {
        private readonly int MinPoints = 1_000;
        private readonly int MaxPoints = 100_000;

        private int TotalPointsValue;      
        public int TotalPoints
        {
            get { return TotalPointsValue; }
            private set
            {
                if (InBounds(value, MinPoints, MaxPoints))
                {
                    TotalPointsValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("totalPoints");
                }
            }
        }

        private readonly int MinClusters = 2;
        private readonly int MaxClusters = 20;

        private int TotalClustersValue; 
        public int TotalClusters {
            get { return TotalClustersValue; }
            private set
            {
                if (InBounds(value, MinClusters, MaxClusters))
                {
                    TotalClustersValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("totalClusters");
                }
            }
        }

        private Point[] PositionsOfPoints;

        private readonly int LowerMaxValueOfX = 100;
        private readonly int LowerMaxValueOfY = 100;

        public int UpperBoundOfX { get; private set; }
        public int UpperBoundOfY { get; private set; }

        public int IterationsDone { get; private set; } = 0;

        /// <summary>
        /// Detects whether passed number is possible
        /// </summary>
        /// <param name="target">Value to check</param>
        /// <param name="lowerBound">Minimum value</param>
        /// <param name="upperBound">Maximum value</param>
        /// <returns>true if target value in passed bounds</returns>
        private bool InBounds(int target, int lowerBound, int upperBound)
        {
            return (target >= lowerBound) && (target <= upperBound);
        }

        public Algorithm(int totalPoints, int totalClusters, int upperXBound, int upperYBound)
            : this()
        {
            TotalPoints = totalPoints;
            TotalClusters = totalClusters;

        }

        private Algorithm()
        {
        }

        public Point[] CreatePointPositions()
        {
            Random random = new Random();

            PositionsOfPoints = new Point[TotalPoints];

            for (int i = 0; i < TotalPoints; i++)
            {
                PositionsOfPoints[i] = new Point(random.Next() % UpperBoundOfX,
                    random.Next() % UpperBoundOfY);
            }

            return (Point[]) PositionsOfPoints.Clone();
        }

        public int[] GenerateClustersCenters()
        {
            for (int i = 0; i < TotalClusters; i++)
            {

            }

            return new int[TotalClusters];
        }

        public void CalculateClusters()
        {

        }

        public void RecalculateClustersCenters()
        {

        }




    }
}
