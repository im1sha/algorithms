using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace Maximin
{
    public class InitialData : IDataErrorInfo, INotifyPropertyChanged
    {

        private string PointsError
        {
            get
            {
                return $"points number should be in following bounds: [ {minPoints}, {maxPoints} ) ";
            }
        }

        private const string TOTAL_POINTS_PROPERTY_AS_STRING = "TotalPoints";

        public const int DEFAULT_IMAGE_SIZE_IN_PIXELS = 1_000;

        private static readonly int minPoints = 1_000;
        private static readonly int maxPoints = 100_000;

        private int totalPoints = maxPoints / 2;
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
                CheckBounds(intValue, minPoints, maxPoints, TOTAL_POINTS_PROPERTY_AS_STRING, PointsError);                
                totalPoints = intValue;
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



