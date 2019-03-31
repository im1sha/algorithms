using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NormalDistribution
{
    public class InteractData : IDataErrorInfo, INotifyPropertyChanged
    {
        public const int DEFAULT_IMAGE_SIZE_IN_PIXELS = 10_000;

        #region probability

        private const string PROBABILITY_PROPERTY_AS_STRING = "TotalPoints";
        private static readonly int minProbability = 0;
        private static readonly int maxProbability = 100;
        private string ProbabilityError
        {
            get
            {
                return $"probability should be in following bounds: [ {minProbability}, {maxProbability} ) ";
            }
        }

        private int probability = maxProbability / 2;
        public string Probability
        {
            get { return probability.ToString(); }
            // checks whether int was passed 
            // if int is out of bounds then CheckBounds() sets an error
            set
            {
                if (!int.TryParse(value, out int intValue))
                {
                    return;
                }
                CheckBounds(intValue, minProbability, maxProbability,
                    PROBABILITY_PROPERTY_AS_STRING, ProbabilityError);
                probability = intValue;
            }
        }

        #endregion

        #region totalpoints

        private const string TOTAL_POINTS_PROPERTY_AS_STRING = "TotalPoints";
        private static readonly int minPoints = 10_000;
        private static readonly int maxPoints = 100_000;
        private string PointsError
        {
            get
            {
                return $"points number should be in following bounds: [ {minPoints}, {maxPoints} ) ";
            }
        }

        private int totalPoints = minPoints;
        /// <summary>
        /// Points number used in the test
        /// </summary>
        public string TotalPoints
        {
            get { return totalPoints.ToString(); }
            // checks whether int was passed 
            // if int is out of bounds then CheckBounds() sets an error
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

        #endregion

        #region IDataErrorInfo and related properties and methods

        /// <summary>
        /// Gets error message of invalid field 
        /// </summary>
        /// <param name="propertyName">Field name</param>
        /// <returns>Error message</returns>
        public string this[string propertyName]
        {
            get
            {
                return !errors.ContainsKey(propertyName) ? null : errors[propertyName];
            }
        }

        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        private bool haveFieldsError = false;
        public bool HaveFieldsError
        {
            get => haveFieldsError;
            private set
            {
                haveFieldsError = value;
                //
            }
        }

        private Dictionary<string, string> errors = new Dictionary<string, string>();

        private void AddError(string propertyName, string error)
        {
            errors[propertyName] = error;
            HaveFieldsError = true;
        }

        private void RemoveError(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
            }
            if (errors.Count == 0)
            {
                HaveFieldsError = false;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

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


        //private bool enabled = true;
        //public bool Enabled
        //{
        //    get => enabled;
        //    set
        //    {
        //        enabled = value;
        //        OnPropertyChanged();
        //    }
        //}
    }
}



