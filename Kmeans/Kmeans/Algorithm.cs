using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kmeans
{
    class Algorithm
    {
        private readonly int MinPoints = 1_000;
        private readonly int MaxPoints = 100_000;
        private int TotalPoints;

        private readonly int MinClusters = 2;
        private readonly int MaxClusters = 20;
        private int TotalClusters ;

        private Point[] PositionsOfPoints;

        private int MaxValueOfX;
        private int MaxValueOfY;

        private int IterationsDone = 0;

        private bool InBounds(int target, int lowerBound, int UpperBound)
        {
            return (target >= lowerBound) && (target <= UpperBound);
        }

        public Algorithm(int totalPoints, int totalClusters): this()
        {
            if (InBounds(totalPoints, MinPoints, MaxPoints))
            {
                TotalPoints = totalPoints;
            }
            else
            {
                throw new ArgumentOutOfRangeException("totalPoints");
            }

            if (InBounds(totalClusters, MinClusters, MaxClusters))
            {
                TotalClusters = totalClusters;
            }
            else
            {
                throw new ArgumentOutOfRangeException("totalClusters");
            }

        }

        private Algorithm()
        {
            MaxValueOfX = 500;
            MaxValueOfY = 500;
        }

        public Point[] CreatePointPositions()
        {
            Random random = new Random();

            PositionsOfPoints = new Point[TotalPoints];

            for (int i = 0; i < TotalPoints; i++)
            {
                PositionsOfPoints[i] = new Point(random.Next() % MaxValueOfX,
                    random.Next() % MaxValueOfY);
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
