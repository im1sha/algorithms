using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kmeans
{
    class Algorithm
    {
        private bool IsInitialized = false;

        private object Sync = new object();

        private readonly int MinPoints = 1_000;
        private readonly int MaxPoints = 100_000;

        private int totalPoints; 
        /// <summary>
        /// Amount of points used in the test
        /// </summary>
        public int TotalPoints
        {
            get { return totalPoints; }
            private set
            {
                if (InBounds(value, MinPoints, MaxPoints))
                {
                    totalPoints = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("totalPoints");
                }
            }
        }

        private readonly int MinClusters = 2;
        private readonly int MaxClusters = 20;

        private int totalClusters;
        /// <summary>
        /// Total amount of clusters to divide points into  
        /// </summary>
        public int TotalClusters
        {
            get { return totalClusters; }
            private set
            {
                if (InBounds(value, MinClusters, MaxClusters))
                {
                    totalClusters = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("totalClusters");
                }
            }
        }

        private readonly int LowerBoundOfMaxValue = 100;
        private readonly int UpperBoundOfMaxValue = 10_000;
      
        private int maxValue; 
        /// <summary>
        /// Upper limit of coordinate value
        /// </summary>
        public int MaxValue
        {
            get { return maxValue; }
            private set
            {
                if (InBounds(value, LowerBoundOfMaxValue, UpperBoundOfMaxValue))
                {
                    maxValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("maxValue");
                }
            }
        }

        /// <summary>
        /// Total iterations done to divide points into clusters 
        /// </summary>
        public int IterationsDone { get; private set; } = 0;

        public List<StaticPoint> PositionsOfPoints { get; private set; } // return positionsOfPoints?.ConvertAll(item => new StaticPoint(item.X, item.Y, item.Cluster));

        public List<ClusterCenter> ClustersCenters { get; private set; }

        /// <summary>
        /// Detects whether passed number is possible
        /// </summary>
        /// <param name="target">Value to check</param>
        /// <param name="lowerBound">Minimum value</param>
        /// <param name="upperBound">Maximum value</param>
        /// <returns>true if target value is in passed bounds, false otherwise</returns>
        private bool InBounds(int target, int lowerBound, int upperBound)
        {
            return (target >= lowerBound) && (target < upperBound);
        }

        public Algorithm(int totalPoints, int totalClusters, int maxValue)
            : this()
        {
            TotalPoints = totalPoints;
            TotalClusters = totalClusters;
            MaxValue = maxValue;
        }

        private Algorithm()
        {
        }

        public void SetInitialClustarization()
        {
            InitializePointsPositions();
            InitializeClustersCenters();
            IsInitialized = true;
        }

        public bool Reclusterize()
        {
            if (!IsInitialized)
            {
                throw new ApplicationException("algorithm data is not initialized");
            }

            CalculateClusters();
            List<ClusterCenter> controlValues = CalculateClustersCenters();
            return IsClusterizationRight(controlValues);
        }

        private void InitializePointsPositions()
        {
            Random random = new Random();
            PositionsOfPoints = new List<StaticPoint>();

            for (int i = 0; i < TotalPoints; i++)
            {
                PositionsOfPoints.Add(new StaticPoint(random.Next() % MaxValue,
                    random.Next() % MaxValue, StaticPoint.NotSpecifiedCluster));
            }
        }

        private void InitializeClustersCenters()
        {
            Random random = new Random();
            ClustersCenters = new List<ClusterCenter>();

            for (int i = 0; i < TotalClusters; i++)
            {
                ClustersCenters.Add(new ClusterCenter(random.Next() % MaxValue,
                    random.Next() % MaxValue, i));
            }
        }

        private void CalculateClusters()
        {
            for (int i = 0; i < PositionsOfPoints.Count; i++)
            {
                int currentMinimalDistance = int.MaxValue;           
                int distanceToCenter;

                foreach (var c in ClustersCenters)
                {
                    distanceToCenter = PositionsOfPoints[i].GetDistanceTo(c);
                    if (distanceToCenter < currentMinimalDistance)
                    {
                        currentMinimalDistance = distanceToCenter;
                        PositionsOfPoints[i].Cluster = c.Index;
                    }
                }               
            }
        }

        private List<ClusterCenter> CalculateClustersCenters()
        {
            List<ClusterCenter> result = new List<ClusterCenter>();

            for (int i = 0; i < TotalClusters; i++)
            {               
                List<StaticPoint> points = PositionsOfPoints.Where(item => item.Cluster == i).ToList();

                result.Add(new ClusterCenter(
                    points.Sum(item => item.X) / points.Count,
                    points.Sum(item => item.Y) / points.Count,
                    i));
            }

            return result;
        }

        private bool IsClusterizationRight(List<ClusterCenter> correctedCenters)
        {
            for (int i = 0; i < ClustersCenters.Count; i++)
            {
                if (ClustersCenters[i] != correctedCenters[i])
                {
                    return false;
                }
            }
           
            return true;
        }
    }
}
