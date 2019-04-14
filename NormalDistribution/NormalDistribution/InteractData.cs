using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace NormalDistribution
{
    public class InteractData : IDataErrorInfo, INotifyPropertyChanged
    {
        public const int DEFAULT_IMAGE_SIZE_IN_PIXELS = 10_000;

        #region probability (%)

        private const string PROBABILITY_PROPERTY_AS_STRING = "Probability";
        private static readonly int minProbability = 0;
        private static readonly int maxProbability = 100;
        private string ProbabilityError
        {
            get
            {
                return $"probability should be in following bounds: [ {minProbability}, {maxProbability} ] ";
            }
        }

        private int probability = maxProbability / 2;
        /// <summary>
        /// %
        /// </summary>
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
                UpdateUI(intValue, minProbability, maxProbability,
                    PROBABILITY_PROPERTY_AS_STRING, ProbabilityError);
                probability = intValue;
            }
        }

        #endregion

        #region total points

        private const string TOTAL_POINTS_PROPERTY_AS_STRING = "TotalPoints";
        private static readonly int minPoints = 1_000;
        private static readonly int maxPoints = 100_000;
        private string PointsError
        {
            get
            {
                return $"points number should be in following bounds: [ {minPoints}, {maxPoints} ] ";
            }
        }

        private int totalPoints = 5_000;
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
                UpdateUI(intValue, minPoints, maxPoints, 
                    TOTAL_POINTS_PROPERTY_AS_STRING, PointsError);                
                totalPoints = intValue;
            }
        }

        #endregion

        #region image set

        private BitmapSource chart;
        public BitmapSource Chart
        {
            get => chart;
            set
            {
                if (chart != value)
                {
                    chart = value;
                    OnPropertyChanged();
                }
            }
        }
        public void SetImage(BitmapSource newValue)
        {
            newValue.Freeze();
            Chart = newValue;
        }

        #endregion

        #region calulations results

        private float falseAlarmError;
        public float FalseAlarmError
        {
            get => falseAlarmError;
            set
            {
                if (value != falseAlarmError)
                {
                    falseAlarmError = value;
                    OnPropertyChanged();
                }
            }
        }
        private float detectionSkipError;
        public float DetectionSkipError
        {
            get => detectionSkipError;
            set
            {
                if (value != detectionSkipError)
                {
                    detectionSkipError = value;
                    OnPropertyChanged();
                }
            }
        }
        private float summaryError;
        public float SummaryError
        {
            get => summaryError;
            set
            {
                if (value != summaryError)
                {
                    summaryError = value;
                    OnPropertyChanged();
                }
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
  
        //private bool haveFieldsError = false;
        //public bool HaveFieldsError
        //{
        //    get => haveFieldsError;
        //    private set
        //    {
        //        haveFieldsError = value;
        //        //
        //    }
        //}

        private Dictionary<string, string> errors = new Dictionary<string, string>();

        private void AddError(string propertyName, string error)
        {
            errors[propertyName] = error;
        }

        private void RemoveError(string propertyName)
        {
            if (errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
            }         
        }

        public string Error { get => string.Empty; }

        /// <summary>
        /// Detects whether passed number is possible
        /// </summary>
        /// <param name="target">Value to check</param>
        /// <param name="lowerBound">Minimum value</param>
        /// <param name="upperBound">Maximum value</param>
        /// <returns>true if target value is in bounds, false otherwise</returns>
        private bool UpdateUI(
            int target,
            int lowerBound,
            int upperBound,
            string propertyNameToChange,
            string errorMessage)
        {
            var result = true;

            if ((target >= lowerBound) && (target <= upperBound))
            {
                RemoveError(propertyNameToChange);
                switch (propertyNameToChange)
                {
                    case PROBABILITY_PROPERTY_AS_STRING:
                        CanProcess = true;
                        break;
                    case TOTAL_POINTS_PROPERTY_AS_STRING:
                        CanGenerate = true;
                        break;
                }
            }
            else
            {
                AddError(propertyNameToChange, errorMessage);
                result = false;

                switch (propertyNameToChange)
                {
                    case PROBABILITY_PROPERTY_AS_STRING:
                        CanProcess = false;
                        break;
                    case TOTAL_POINTS_PROPERTY_AS_STRING:
                        CanGenerate = false;
                        break;
                }            
            }

            return result;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion

        // true after model initialization
        private bool isInitialized = false;
        public bool IsInitialized {
            get => isInitialized;
            set {
                isInitialized = true;
                CanProcess = true;
            }
        }  

        private bool canGenerate = true;
        public bool CanGenerate
        {
            get => canGenerate;
            set
            {
                if (canGenerate != value)
                {
                    if (value == true && errors.ContainsKey(TOTAL_POINTS_PROPERTY_AS_STRING))
                    {
                        return;
                    } 
                    canGenerate = value;
                    OnPropertyChanged();                                                       
                }                    
            }
        } 

        private bool canProcess = false;
        public bool CanProcess
        {
            get => canProcess;
            set
            {
                if (canProcess != value)
                {
                    if (value == true && errors.ContainsKey(PROBABILITY_PROPERTY_AS_STRING))
                    {
                        return;
                    }                                     
                    canProcess = value && IsInitialized;
                    OnPropertyChanged();
                }
            }
        }
    }
}



