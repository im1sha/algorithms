using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Kmeans
{
    public class InitialData : IDataErrorInfo, INotifyPropertyChanged
    {     
        private readonly uint[] defaultColors = new[] {
            0xFF044b42,
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
                int totalColors = int.Parse(TotalClusters);

                if (totalColors > defaultColors.Length)
                {
                    throw new ArgumentException();
                }
                uint[] colors = new uint[totalColors];

                for (int i = 0; i < totalColors; i++)
                {
                    colors[i] = defaultColors[i];
                }

                return colors;
            }
        }

        private string PointsError
        {
            get
            {
                return $"points number should be in following bounds: [{MinPoints}, {MaxPoints}) ";
            }
        }

        private string ClustersError
        {
            get
            {
                return $"clusters number should be in following bounds: [{MinClusters}, {MaxClusters})";
            }
        }

        private const string TotalPointsPropertyAsString = "TotalPoints";
        private const string TotalClustersPropertyAsString = "TotalClusters";

        public const int DefaultImageSizeInPixels = 1_000;

        private static readonly int MinPoints = 1_000;
        private static readonly int MaxPoints = 100_000;

        private int totalPoints = MaxPoints / 2;
        /// <summary>
        /// Points number used in the test
        /// </summary>
        public string TotalPoints
        {
            get { return totalPoints.ToString(); }
            set
            {
                if (!int.TryParse(value, out int intValue))
                {
                    return;
                }
                CheckBounds(intValue, MinPoints, MaxPoints, TotalPointsPropertyAsString, PointsError);                
                totalPoints = intValue;
            }
        }

        private static readonly int MinClusters = 2;
        private static readonly int MaxClusters = 20;

        private int totalClusters = MaxClusters / 2;
        /// <summary>
        /// Amount of clusters to divide points into  
        /// </summary>
        public string TotalClusters
        {
            get { return totalClusters.ToString(); }
            set
            {
                if (!int.TryParse(value, out int intValue))
                {
                    return;
                }
                CheckBounds(intValue, MinClusters, MaxClusters, TotalClustersPropertyAsString, ClustersError);
                totalClusters = intValue;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                return !errors.ContainsKey(propertyName) ? null : errors[propertyName];
            }
        }

        /// <summary>
        /// Detects whether passed number is possible
        /// </summary>
        /// <param name="target">Value to check</param>
        /// <param name="lowerBound">Minimum value</param>
        /// <param name="upperBound">Maximum value</param>
        /// <returns>true if target value is in passed bounds, false otherwise</returns>
        private bool CheckBounds(int target, int lowerBound, int upperBound,
            string propertyNameToChange, string errorMessage)
        {
            if ((target >= lowerBound) && (target < upperBound))
            {
                RemoveError(propertyNameToChange);
                return true;
            }

            AddError(propertyNameToChange, errorMessage);
            return false;
        }

        private Dictionary<string, string> errors = new Dictionary<string, string>();

        private void AddError(string propertyName, string error)
        {
            errors[propertyName] = error;
            Enabled = false;
        }

        private void RemoveError(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
            }
            if (errors.Count == 0)
            {
                Enabled = true;
            }
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        private bool enabled = true;
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

      
    }
}



