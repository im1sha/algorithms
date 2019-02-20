using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans
{

    public class InitialData : IDataErrorInfo
    {
        private uint[] defaultColors = new[] {
            0xFFf44b42,
            0xFFf47141,
            0xFFf4ac41,
            0xFFf4d641,
            0xFFe2f441,
            0xFF79f441,
            0xFF41f4a9,
            0xFF41f4f4,
            0xFF419df4,
            0xFF414cf4,
            0xFF7c41f4,
            0xFFa941f4,
            0xFF431663,
            0xFFf141f4,
            0xFFfa00ff,
            0xFFf4419a,
            0xFFf4beb7,
            0xFFdcf4b7,
            0xFFb7e5f4,
            0xFF747474,
        };

        public uint[] Colors
        {
            get
            { 
                uint[] colors = new uint[TotalClusters];

                for (int i = 0; i < TotalClusters; i++)
                {
                    colors[i] = defaultColors[i];
                }

                return colors;
            }
        }
      

        private const string TotalPointsPropertyAsString = "TotalPoints";
        private const string TotalClustersPropertyAsString = "TotalClusters";

        public const int DefaultImageSizeInPixels = 1_000;

        private static readonly int MinPoints = 1_000;
        private static readonly int MaxPoints = 100_000;


        /// <summary>
        /// Points number used in the test
        /// </summary>
        public int TotalPoints { get; set; } = MaxPoints - 1;

        private static readonly int MinClusters = 2;
        private static readonly int MaxClusters = 20;

        /// <summary>
        /// Amount of clusters to divide points into  
        /// </summary>
        public int TotalClusters { get; set; } = MaxClusters - 1;

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



