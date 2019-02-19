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
        public readonly int MaxCoordinate;

        public bool IsInitialized { get; private set; }
       
        /// <summary>
        /// Total iterations done to divide points into clusters 
        /// </summary>
        public int IterationsDone { get; private set; } = 0;

        public (StaticPoint Center, List<StaticPoint> StaticPoints)[] Clusters { get; private set; }
        private (StaticPoint Center, List<StaticPoint> StaticPoints)[] NewIterationClusters;


        public Algorithm(int totalPoints, int totalClusters, int maxCoordinate)
        {
            TotalPoints = totalPoints;
            TotalClusters = totalClusters;
            MaxCoordinate = maxCoordinate;
        }


        public void SetInitialClustarization()
        {
            Random random = new Random();

            // initialize centers
            for (int i = 0; i < TotalClusters; i++)
            {           
                Clusters[i] = (new StaticPoint(random.Next() % MaxCoordinate, 
                    random.Next() % MaxCoordinate, i), new List<StaticPoint>());
            }

            // generate other points
            for (int i = TotalClusters; i < TotalPoints; i++)
            {
                Clusters[0].StaticPoints.Add(new StaticPoint(random.Next() % MaxCoordinate,
                    random.Next() % MaxCoordinate, 0));
            }

            IsInitialized = true;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Array of clusters or null if clusters have no changes</returns>
        public (StaticPoint Сenter, List<StaticPoint> StaticPoints)[] Reclusterize()
        {
            if (!IsInitialized)
            {
                throw new ApplicationException("Algorithm data is not initialized");
            }

            NewIterationClusters = CalculateClusters(Clusters);
            StaticPoint[] controlValues = RecalculateClustersCenters(NewIterationClusters);

            if (!IsClusterizationRight(controlValues))
            {
                Clusters = NewIterationClusters;
                return Clusters;
            }

            return null;
        }

        private (StaticPoint Center, List<StaticPoint> StaticPoints)[] GetCentersAndEmptyClusters(
            (StaticPoint Center, List<StaticPoint> StaticPoints)[] source)
        {
            int length = source.Length;

            var result  = new (StaticPoint Center,
                List<StaticPoint> StaticPoints)[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = (StaticPoint.Copy(source[i].Center), new List<StaticPoint>());
            }

            return result;
        }

        private List<StaticPoint> GetCentersCopy((StaticPoint Center, List<StaticPoint> StaticPoints)[] source)
        {
            return source.Select(item => source.ElementAt(item.Center.ClusterIndex).Center).ToList();
        }


        /// <summary>
        /// Recalculates clusters for given centers by placing StaticPoint's instances 
        /// to the nearest center's cluster 
        /// </summary>
        /// <param name="clusters">Initial array</param>
        /// <returns>Recalculated clusters array</returns>
        private (StaticPoint Center, List<StaticPoint> StaticPoints)[] CalculateClusters( 
            (StaticPoint Center, List<StaticPoint> StaticPoints)[] clusters)
        {
            (StaticPoint Center, List<StaticPoint> StaticPoints)[] result =
                GetCentersAndEmptyClusters(clusters);

            for (int i = 0; i < clusters.Length; i++)
            {
                for (int j = 0; j < clusters[i].StaticPoints.Count; j++)
                {
                    int nearestCenterIndex = GetNearestPointIndex(clusters[i].StaticPoints[j], 
                        GetCentersCopy(clusters));

                    result[nearestCenterIndex].StaticPoints.Add(clusters[i].StaticPoints[j]);
                }
            }

            return result;
        }

        private int GetNearestPointIndex(StaticPoint point, List<StaticPoint> staticPoints)
        {
            int currentMinimalDistance = int.MaxValue;
            int nearestPointIndex = 0;
            int distanceToTargetPoint;

            for (int i = 0; i < staticPoints.Count; i++)
            {
                distanceToTargetPoint = point.GetDistanceTo(staticPoints[i]);
                if (distanceToTargetPoint < currentMinimalDistance)
                {
                    currentMinimalDistance = distanceToTargetPoint;
                    nearestPointIndex = i;
                }
            }

            return nearestPointIndex;
        }

        private StaticPoint[] RecalculateClustersCenters(
            (StaticPoint Center, List<StaticPoint> StaticPoints)[] cluster)
        {
            var arithmeticCenters = new StaticPoint[TotalClusters];
            var nearestPoints = new StaticPoint[TotalClusters];
            int nearestPointIndex = 0;

            for (int i = 0; i < TotalClusters; i++)
            {            
                // move center to StaticPoints
                cluster[i].StaticPoints.Add(new StaticPoint(
                    cluster[i].Center.X,
                    cluster[i].Center.Y,
                    cluster[i].Center.ClusterIndex));

                // calculate new central points
                arithmeticCenters[i] = new StaticPoint(
                    cluster[i].StaticPoints.Sum(item => item.X) / cluster[i].StaticPoints.Count,
                    cluster[i].StaticPoints.Sum(item => item.Y) / cluster[i].StaticPoints.Count,
                    i);

                // get nearest point index
                nearestPointIndex = GetNearestPointIndex(arithmeticCenters[i], cluster[i].StaticPoints);


                nearestPoints[i] = StaticPoint.Copy(cluster[i].StaticPoints[nearestPointIndex]);

                // move new cluster's center from StaticPoints to Center
                cluster[i].Center = StaticPoint.Copy(cluster[i].StaticPoints[nearestPointIndex]);
                cluster[i].StaticPoints.RemoveAt(nearestPointIndex);

            }

            return nearestPoints;
        }

        private bool IsClusterizationRight(StaticPoint[] correctedCenters)
        {
            List<StaticPoint> oldCenters = GetCentersCopy(Clusters);

            for (int i = 0; i < correctedCenters.Length; i++)
            {
                if (oldCenters[i] != correctedCenters[i])
                {
                    return false;
                }
            }
           
            return true;
        }
    }
}


