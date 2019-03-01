using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maximin
{
    class Maximin
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
                    throw new ApplicationException("Algorithm data is not initialized.");
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

        // center is prototype
        private List<(StaticPoint Center, List<StaticPoint> StaticPoints)> CurrentClusters;

        public Maximin(int totalPoints, int maxCoordinate)
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

            CurrentClusters = new List<(StaticPoint Center, List<StaticPoint> StaticPoints)>
            {
                // initialize center
                (new StaticPoint(random.Next() % MaxCoordinate, random.Next() % MaxCoordinate), 
                    new List<StaticPoint>())
            };

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

            // get farthest point for each the prototype
            (int distance, StaticPoint farthestPoint)[] farthestPoints = 
                new (int, StaticPoint)[CurrentClusters.Count];
            for (int i = 0; i < farthestPoints.Length; i++)
            {
                int indexOfFarthestPoint = CurrentClusters[i].Center.GetFarthestPointIndex(
                    CurrentClusters[i].StaticPoints.ToArray());

                farthestPoints[i].farthestPoint = CurrentClusters[i].StaticPoints[indexOfFarthestPoint];
                farthestPoints[i].distance = farthestPoints[i].farthestPoint.GetSquareOfDistanceTo(
                    CurrentClusters[i].Center);
            }

            int thredshold = GetThredshold(CentersToArray(CurrentClusters));
                      
            // get new prototype 
            StaticPoint? newPrototype = GetNewPrototype(farthestPoints, thredshold);

            if (newPrototype != null)
            {
                // recalculate clusters if new prototype added
                CurrentClusters = CalculateClusters(CurrentClusters, (StaticPoint)newPrototype);
                return GetClustersOutputCopy(CurrentClusters);            
            }

            IsFinalState = true;

            return null;
        }

        private int GetThredshold(StaticPoint[] centers)
        {
            if (centers == null || centers.Length == 0)
            {
                throw new ArgumentException("No centers.");
            }
            else if (centers.Length == 1)
            {
                return 0;
            }

            int result = 0;
            if (centers.Length > 1)
            {
                int totalDistances = 0;
                int totalLength = 0;
                for (int i = 0; i < centers.Length - 1; i++)
                {
                    for (int j = i + 1; j < centers.Length; j++)
                    {
                        totalDistances++;
                        totalLength += centers[i].GetSquareOfDistanceTo(centers[j]);
                    }
                }
                result = totalLength / (2 * totalDistances);
            }

            return result;
        }

        private StaticPoint? GetNewPrototype((int distance, StaticPoint farthestPoint)[] farthestPoints, int thredshold)
        {
            int indexOfFarthestPoint = 0;

            for (int i = 0; i < farthestPoints.Length; i++)
            {            
                if (farthestPoints[i].distance > farthestPoints[indexOfFarthestPoint].distance)
                {
                    indexOfFarthestPoint = i; 
                }
            }

            if (farthestPoints[indexOfFarthestPoint].distance < thredshold)
            {
                return null;
            }

            return farthestPoints[indexOfFarthestPoint].farthestPoint;
        }

        private (StaticPoint Center, StaticPoint[] StaticPoints)[] GetClustersOutputCopy(
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

        private List<(StaticPoint Center, List<StaticPoint> StaticPoints)> GetCentersAndEmptyClustersCopy(
            List<(StaticPoint Center, List<StaticPoint> StaticPoints)> source)
        {
            int length = source.Count;

            var result = new List<(StaticPoint Center, List<StaticPoint> StaticPoints)>();

            for (int i = 0; i < length; i++)
            {
                result.Add((source[i].Center, new List<StaticPoint>()));
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
            List<(StaticPoint Center, List<StaticPoint> StaticPoints)> clusters, StaticPoint newPrototype)
        {
            List<(StaticPoint Center, List<StaticPoint> StaticPoints)> result =
                GetCentersAndEmptyClustersCopy(clusters);

            result.Add((newPrototype, new List<StaticPoint> ()));

            StaticPoint[] centersCopy = CentersToArray(result);

            for (int i = 0; i < clusters.Count; i++)
            {
                for (int j = 0; j < clusters[i].StaticPoints.Count; j++)
                {
                    int nearestCenterIndex = clusters[i].StaticPoints[j].GetNearestPointIndex(centersCopy);
                    result[nearestCenterIndex].StaticPoints.Add(clusters[i].StaticPoints[j]);
                }
            }
            return result;
        }
    }
}


