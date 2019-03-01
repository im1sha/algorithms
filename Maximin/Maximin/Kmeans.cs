using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maximin
{
    class Kmeans
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

        public bool IsFinalState { get; private set; }

        private (StaticPoint Center, List<StaticPoint> StaticPoints)[] CurrentClusters;
        private (StaticPoint Center, List<StaticPoint> StaticPoints)[] NewIterationClusters;


        public Kmeans(int totalClusters, int totalPoints, int maxCoordinate)
        {
            TotalPoints = totalPoints;
            TotalClusters = totalClusters;
            MaxCoordinate = maxCoordinate;
            NewIterationClusters = new (StaticPoint Center, List<StaticPoint> StaticPoints)[TotalClusters];
        }

        /// <summary>
        /// Initilizes data required by algorithm
        /// </summary>
        public void SetInitialClustarization()
        {
            if (IsInitialized)
            {
                throw new ApplicationException("Initialized second time.");
            }

            Random random = new Random();

            CurrentClusters = new (StaticPoint Center, List<StaticPoint> StaticPoints)[TotalClusters];

            // initialize centers
            for (int i = 0; i < TotalClusters; i++)
            {           
                CurrentClusters[i] = (new StaticPoint(random.Next() % MaxCoordinate, 
                    random.Next() % MaxCoordinate), new List<StaticPoint>());
            }

            // generate other points
            for (int i = TotalClusters; i < TotalPoints; i++)
            {
                CurrentClusters[0].StaticPoints.Add(new StaticPoint(random.Next() % MaxCoordinate,
                    random.Next() % MaxCoordinate));
            }

            IsInitialized = true;            
        }

        public void SetInitialClustarization((StaticPoint Center, StaticPoint[] StaticPoints)[] clusters)
        {
            if (IsInitialized)
            {
                throw new ApplicationException("Initialized second time.");
            }

            CurrentClusters = clusters.Select(i => (i.Center, i.StaticPoints.ToList())).ToArray();

            IsInitialized = true;
        }

        /// <summary>
        /// Reclusterizes points if they are not in final clustarization state
        /// </summary>
        /// <returns>Array of clusters or null if clusters have no changes</returns>
        public (StaticPoint Сenter, StaticPoint[] StaticPoints)[] Reclusterize()
        {
            if (!IsInitialized)
            {
                throw new ApplicationException("Algorithm data is not initialized.");
            }

            // correct clusters's positions 
            NewIterationClusters = CalculateClusters(CurrentClusters);
            StaticPoint[] controlValues = RecalculateCenters(ref NewIterationClusters);

            if (!IsClusterizationRight(controlValues, CentersToList(CurrentClusters).ToArray()))
            {
                CurrentClusters = NewIterationClusters;
                NewIterationClusters = new (StaticPoint Center, List<StaticPoint> StaticPoints)[TotalClusters];
                return GetClustersOutputCopy(CurrentClusters);
            }
            IsFinalState = true;
            return null;
        }

        private (StaticPoint Center, StaticPoint[] StaticPoints)[] GetClustersOutputCopy(
            (StaticPoint Center, List<StaticPoint> StaticPoints)[] source)
        {
            (StaticPoint Center, StaticPoint[] StaticPoints)[] result = new
                (StaticPoint Center, StaticPoint[] StaticPoints)[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                result[i] = (source[i].Center, source[i].StaticPoints.ToArray());
            }
            return result;
        }

        private (StaticPoint Center, List<StaticPoint> StaticPoints)[] GetCentersAndEmptyClustersCopy(
            (StaticPoint Center, List<StaticPoint> StaticPoints)[] source)
        {
            int length = source.Length;

            var result  = new (StaticPoint Center,
                List<StaticPoint> StaticPoints)[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = (source[i].Center, new List<StaticPoint>());
            }

            return result;
        }

        private List<StaticPoint> CentersToList((StaticPoint Center, List<StaticPoint> StaticPoints)[] source)
        {
            var result = new List<StaticPoint>();
            foreach (var item in source)
            {
                result.Add(item.Center);
            }
            return result;
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
                GetCentersAndEmptyClustersCopy(clusters);

            List<StaticPoint> centersCopy = CentersToList(clusters);

            for (int i = 0; i < clusters.Length; i++)
            {
                for (int j = 0; j < clusters[i].StaticPoints.Count; j++)
                {
                    int nearestCenterIndex = clusters[i].StaticPoints[j].GetNearestPointIndex(centersCopy.ToArray());

                    result[nearestCenterIndex].StaticPoints.Add(clusters[i].StaticPoints[j]);
                }
            }

            return result;
        }

        /// <summary>
        /// Corrects clusters's centers
        /// </summary>
        /// <param name="cluster">Clusters to analyze</param>
        /// <returns>Correct centers positions</returns>
        private StaticPoint[] RecalculateCenters(ref 
            (StaticPoint Center, List<StaticPoint> StaticPoints)[] cluster)
        {
            var arithmeticCenters = new StaticPoint[TotalClusters];
            var nearestPoints = new StaticPoint[TotalClusters];
            int nearestPointIndex = 0;

            for (int i = 0; i < TotalClusters; i++)
            {            
                // move center to StaticPoints
                cluster[i].StaticPoints.Insert(0, new StaticPoint(
                    cluster[i].Center.X,
                    cluster[i].Center.Y));

                // calculate new central points
                arithmeticCenters[i] = new StaticPoint(
                    cluster[i].StaticPoints.Sum(item => item.X) / cluster[i].StaticPoints.Count,
                    cluster[i].StaticPoints.Sum(item => item.Y) / cluster[i].StaticPoints.Count);

                // get nearest point by index
                nearestPointIndex = arithmeticCenters[i].GetNearestPointIndex(cluster[i].StaticPoints.ToArray());
                nearestPoints[i] = cluster[i].StaticPoints[nearestPointIndex];

                // move new cluster's center from StaticPoints to Center
                cluster[i].Center = cluster[i].StaticPoints[nearestPointIndex];
                cluster[i].StaticPoints.RemoveAt(nearestPointIndex);
            }

            return nearestPoints;
        }

        private bool IsClusterizationRight(StaticPoint[] correctedCenters, StaticPoint[] oldCenters)
        {
            if (correctedCenters.Length != oldCenters.Length)
            {
                throw new ApplicationException("Not equal length of parameters.");
            }

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


