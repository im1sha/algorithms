﻿using System;
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
        public int TotalClusters {
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

        private Point[] positionsOfPoints;
        public Point[] PositionsOfPoints
        {
            get { return (Point[]) positionsOfPoints?.Clone(); }
        }

        private Point[] clustersCenters;
        public Point[] ClustersCenters
        {
            get { return (Point[]) clustersCenters?.Clone(); }
        }

        /// <summary>
        /// Detects whether passed number is possible
        /// </summary>
        /// <param name="target">Value to check</param>
        /// <param name="lowerBound">Minimum value</param>
        /// <param name="upperBound">Maximum value</param>
        /// <returns>true if target value in passed bounds, false otherwise</returns>
        private bool InBounds(int target, int lowerBound, int upperBound)
        {
            return (target >= lowerBound) && (target <= upperBound);
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

        /// <summary>
        /// 
        /// </summary>
        public void SetInitialClustarization()
        {
            InitializePointsPositions();
            InitializeClustersCenters();
        }

        public bool Reclusterize()
        {
            return true;
        }

        private void InitializePointsPositions()
        {
            InitializePoints(ref positionsOfPoints, TotalPoints, MaxValue);
        }

        private void InitializeClustersCenters()
        {
            InitializePoints(ref clustersCenters, TotalClusters, MaxValue);
        }

        private void InitializePoints(ref Point[] points, int totalPoints, int upperValue)
        {
            Random random = new Random();
            points = new Point[totalPoints];

            for (int i = 0; i < totalPoints; i++)
            {
                points[i] = new Point(random.Next() % upperValue,
                    random.Next() % upperValue);
            }
        }

        private void CalculateClusters()
        {

        }

        private void CalculateClustersCenters()
        {

        }
    }
}
