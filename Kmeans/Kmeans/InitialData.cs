﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans
{

    public class InitialData : IDataErrorInfo
    {
        private const string TotalPointsPropertyAsString = "TotalPoints";
        private const string TotalClustersPropertyAsString = "TotalClusters";

        // private bool IsInitialized = false;

        private static readonly int MinPoints = 1_000;
        private static readonly int MaxPoints = 100_000;


        /// <summary>
        /// Points number used in the test
        /// </summary>
        public int TotalPoints { get; set; } = MinPoints;

        private static readonly int MinClusters = 2;
        private static readonly int MaxClusters = 20;

        /// <summary>
        /// Amount of clusters to divide points into  
        /// </summary>
        public int TotalClusters { get; set; } = MinClusters;

        public string this[string fieldName]
        {
            get
            {
                string error = string.Empty;
                switch (fieldName)
                {
                    case TotalPointsPropertyAsString:
                        if (!InBounds(TotalPoints, MinPoints, MaxPoints))
                        {
                            error = $"points number should be in following bounds: [{MinPoints}, {MaxPoints}) ";
                        }
                        break;
                    case TotalClustersPropertyAsString:
                        if (!InBounds(TotalClusters, MinClusters, MaxClusters))
                        {
                            error = $"clusters number should be in following bounds: [{MinClusters}, {MaxClusters})";
                        }
                        break;
                }

                if (error != string.Empty)
                {
                    Error = error;
                }

                return error;
            }
        }

        public string Error { get; private set; } = string.Empty;

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
    }
}
