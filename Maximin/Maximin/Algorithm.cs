using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maximin
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
        public int TotalClusters
        {
            get
            {
                if (CurrentClusters == null)
                {
                    return 0;
                }
                return CurrentClusters.Count;
            }
        }

        /// <summary>
        /// Field size
        /// </summary>
        public readonly int MaxCoordinate;

        public bool IsInitialized { get; private set; }

        public bool IsFinalState { get; private set; }

        private List<(StaticPoint Center, List<StaticPoint> StaticPoints)> CurrentClusters;
        private List<(StaticPoint Center, List<StaticPoint> StaticPoints)> NewIterationClusters;

        public Algorithm(int totalPoints, int maxCoordinate)
        {
            TotalPoints = totalPoints;
            MaxCoordinate = maxCoordinate;
        }

        /// <summary>
        /// Initilizes data required by algorithm
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
            {
                throw new ApplicationException("Initialized second time.");
            }

            Random random = new Random();

            CurrentClusters = new List<(StaticPoint Center, List<StaticPoint> StaticPoints)> ();
            NewIterationClusters = new List<(StaticPoint Center, List<StaticPoint> StaticPoints)> ();

            // initialize center
            CurrentClusters.Add((new StaticPoint(random.Next() % MaxCoordinate, 
                random.Next() % MaxCoordinate), new List<StaticPoint> ()));

            // generate other points
            for (int i = 1; i < TotalPoints; i++)
            {
                CurrentClusters[0].StaticPoints.Add(new StaticPoint(random.Next() % MaxCoordinate,
                    random.Next() % MaxCoordinate));
            }

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

            // center is prototype
            // 
            // 1 get_threadshold ([] prototypes){} 
            //      returns 0 for 1 point
            //      exception if less than 1
            // 2 get farest points foreach prototype
            // 3 get new ptototype(List<> farestpoints) 
            //      return farest if exists
            //      return null otherwise
            // 4 recalculate clusters

            //// correct clusters's positions 
            //NewIterationClusters = CalculateClusters(CurrentClusters);

            //StaticPoint[] controlValues = RecalculateCenters(ref NewIterationClusters);

            //if (!IsClusterizationRight(controlValues, CentersToList(CurrentClusters).ToArray()))
            //{
            //    CurrentClusters = NewIterationClusters;
            //    NewIterationClusters = new List<(StaticPoint Center, List<StaticPoint> StaticPoints)>();
            //    return GetClustersCopy(CurrentClusters);
            //}
            //IsFinalState = true;
            return null;
        }

        private int GetThreadshold(StaticPoint[] centers)
        {
            throw new NotImplementedException();
        }

        private (StaticPoint Center, StaticPoint[] StaticPoints)[] GetClustersCopy(
            List<(StaticPoint Center, List<StaticPoint> StaticPoints)> source)
        {
            (StaticPoint Center, StaticPoint[] StaticPoints)[] result = new
                (StaticPoint Center, StaticPoint[] StaticPoints)[source.Count];

            for (int i = 0; i < source.Count; i++)
            {
                result[i] = (source[i].Center, source[i].StaticPoints.ToArray());
            }

            return result;
        }

        private StaticPoint[] CentersToArray(List<(StaticPoint Center, List<StaticPoint> StaticPoints)> source)
        {
            var result = new StaticPoint[source.Count];

            for (int i = 0; i < source.Count; i++)
            {
                result[i] = source[i].Center;
            }

            return result;
        }

        private int GetNearestPointIndex(StaticPoint point, StaticPoint[] pointsToCompare)
        {
            int currentMinimalDistance = int.MaxValue;
            int nearestPointIndex = 0;
            int distanceToTargetPoint;

            for (int i = 0; i < pointsToCompare.Length; i++)
            {
                distanceToTargetPoint = point.GetSquareOfDistanceTo(pointsToCompare[i]);
                if (distanceToTargetPoint < currentMinimalDistance)
                {
                    currentMinimalDistance = distanceToTargetPoint;
                    nearestPointIndex = i;
                }
            }

            return nearestPointIndex;
        }

        private List<(StaticPoint Center, List<StaticPoint> StaticPoints)> GetCentersAndEmptyClustersCopy(
            List<(StaticPoint Center, List<StaticPoint> StaticPoints)> source)
        {
            int length = source.Count;

            var result = new List<(StaticPoint Center, List<StaticPoint> StaticPoints)>();

            for (int i = 0; i < length; i++)
            {
                result[i] = (source[i].Center, new List<StaticPoint>());
            }

            return result;
        }

        /// <summary>
        /// Recalculates clusters for given centers by placing StaticPoint's instances 
        /// to the nearest center's cluster 
        /// </summary>
        /// <param name="clusters">Initial array</param>
        /// <returns>Recalculated clusters array</returns>
        private List<(StaticPoint Center, List<StaticPoint> StaticPoints)> CalculateClusters(
            List<(StaticPoint Center, List<StaticPoint> StaticPoints)> clusters)
        {
            List<(StaticPoint Center, List<StaticPoint> StaticPoints)> result =
                GetCentersAndEmptyClustersCopy(clusters);

            StaticPoint[] centersCopy = CentersToArray(clusters);

            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = 0; j < clusters[i].StaticPoints.Count; j++)
                {
                    int nearestCenterIndex = GetNearestPointIndex(clusters[i].StaticPoints[j], centersCopy);

                    result[nearestCenterIndex].StaticPoints.Add(clusters[i].StaticPoints[j]);
                }
            }
            return result;
        }



    }
}


