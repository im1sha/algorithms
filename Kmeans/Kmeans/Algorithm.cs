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
        /// <summary>
        /// Number of points used in the test
        /// </summary>
        public readonly int TotalPoints;

        /// <summary>
        /// Total amount of clusters to divide points into  
        /// </summary>
        public readonly int TotalClusters;

        /// <summary>
        /// Field size
        /// </summary>
        public readonly int MaxCoordinate = 1_000;


        public bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Total iterations done to divide points into clusters 
        /// </summary>
        public int IterationsDone { get; private set; } = 0;

        public Dictionary<int, (ClusterCenter center, List<StaticPoint> staticPoints) > PointsDictionary { get; private set; }


        public Algorithm(int totalPoints, int totalClusters)
        {
            TotalPoints = totalPoints;
            TotalClusters = totalClusters;
        }


        public void SetInitialClustarization()
        {
            Random random = new Random();

            // initialize centers
            for (int i = 0; i < TotalClusters; i++)
            {           
                PointsDictionary.Add(i, (new ClusterCenter(random.Next() % MaxCoordinate, 
                    random.Next() % MaxCoordinate, i), new List<StaticPoint>()));
            }

            // generate other points
            for (int i = TotalClusters; i < TotalPoints; i++)
            {
                PointsDictionary[0].staticPoints.Add(new StaticPoint(random.Next() % MaxCoordinate,
                    random.Next() % MaxCoordinate, 0));
            }

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
