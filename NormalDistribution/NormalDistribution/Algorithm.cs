using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormalDistribution
{
    class Algorithm
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsCalculated { get; private set; } = false;

        public readonly int MaxPointPosition;
        public readonly int TotalPoints;

        /// <summary>
        /// decisionRuleValues is ( prior_probability * posterior_probability ), 
        /// mean - матожидание,  
        /// standardDeviation - среднее кв отклонение  
        /// </summary>
        private ((double point, double decisionRuleValue)[] values, double mean, double standardDeviation)[] data
            = new ((double point, double decisionRuleValue)[] values, double mean, double standardDeviation)[2];

        #region Calculactions results

        private double falseAlarmError; // вероятность ложной тревоги
        public double FalseAlarmError {
            get { ThrowIfNotCalculated(); return falseAlarmError; }
            private set { falseAlarmError = value; }
        }

        private double detectionSkipError; // вероятность пропуска ошибки
        public double DetectionSkipError {
            get { ThrowIfNotCalculated(); return detectionSkipError; }
            private set { detectionSkipError = value; }
        }

        public double Error { get { ThrowIfNotCalculated(); return DetectionSkipError + FalseAlarmError; } }

        private (double point, double decisionRuleValue) valueOfMinimumErrorPoint; // intersection of decisionRuleValues[0] and decisionRuleValues[1]
        public (double point, double decisionRuleValue) ValueOfMinimumErrorPoint {
            get { ThrowIfNotCalculated(); return valueOfMinimumErrorPoint; }
            private set { valueOfMinimumErrorPoint = value; }
        }

        public (double point, double decisionRuleValue)[][] Values
        {
            get {
                ThrowIfNotCalculated();
                return new (double point, double decisionRuleValue)[][] {
                   ((double point, double decisionRuleValue)[]) data[0].values.Clone(),
                   ((double point, double decisionRuleValue)[]) data[1].values.Clone()
                };
            }
        }

        private void ThrowIfNotCalculated()
        {
            if (!IsCalculated)
            {
                throw new ApplicationException("Not calculated.");
            }
        }

        #endregion

        /// <param name="totalPoints">Single sequence length</param>
        /// <param name="maxPointPosition">Max value of point</param>
        public Algorithm(int totalPoints, int maxPointPosition)
        {
            MaxPointPosition = maxPointPosition;
            TotalPoints = totalPoints;
        }

        #region Initialization

        private (double leftOfSecond, double rightOfFirst) GetBounds()
        {
            Random random = new Random();

            double leftOfSecond = random.Next(0, MaxPointPosition / 2);
            double rightOfFirst = random.Next(MaxPointPosition / 2, MaxPointPosition);

            return (leftOfSecond, rightOfFirst);
        }

        private double[] GetSortedSequence(double leftBound, double rightBound, int totalPoints)
        {
            double interval = rightBound - leftBound;

            Random r = new Random();
            var result = new double[totalPoints];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = r.NextDouble() * interval + leftBound;
            }
            Array.Sort(result);
            return result;
        }

        private double GetMean(double[] sequence)
        {
            return sequence.Sum() / sequence.Length;
        }

        private double GetStandardDeviation(double[] sequence, double mean)
        {
            return Math.Sqrt(sequence.Select(x => Math.Pow(x - mean, 2)).Sum() / sequence.Length);
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates posterior probability for normal distribution
        /// </summary>
        private double GetPosteriorProbability(double x, double mean, double standardDeviation)
        {
            return Math.Exp(-0.5 * Math.Pow((x - mean) / standardDeviation, 2))
                 / (standardDeviation * Math.Sqrt(2 * Math.PI));
        }

        /// <summary>
        /// Values used to determine class which point belongs
        /// </summary>
        private double GetDecisionRuleValue(double priorProbability, double posteriorProbability)
        {
            return priorProbability * posteriorProbability;
        }

        /// <summary>
        /// Calculate DecisionRuleValue for each item of the sequence
        /// </summary>
        private double[] CalculateDecisionRuleValuesSequence(double[] sequence, double priorProbability,
            double mean, double standardDeviation)
        {
            return sequence.Select(x => GetDecisionRuleValue(priorProbability, GetPosteriorProbability(x, mean, standardDeviation))).ToArray();
        }

        private (double point, double decisionRuleValue) CalculateValueOfMinimumErrorPoint(
            (double point, double decisionRuleValue)[] values1,
            (double point, double decisionRuleValue)[] values2)
        {
            double minimalSquareOfDistance = double.MaxValue;
            double squareOfCurrentDistance;

            // nearest ones
            (double point, double decisionRuleValue) result1 = (0, 0);
            (double point, double decisionRuleValue) result2 = (0, 0);

            foreach (var item1 in values1)
            {
                foreach (var item2 in values2)
                {
                    squareOfCurrentDistance = Math.Pow(item1.decisionRuleValue - item2.decisionRuleValue, 2)
                        + Math.Pow(item1.point - item2.point, 2);

                    if (squareOfCurrentDistance < minimalSquareOfDistance)
                    {
                        minimalSquareOfDistance = squareOfCurrentDistance;
                        result1 = item1;
                        result2 = item2;
                    }
                }
            }

            return ((result1.point + result2.point) / 2, (result1.decisionRuleValue + result2.decisionRuleValue) / 2);
        }

        private double CalculateFalseAlarmError(
            (double point, double decisionRuleValue)[] values,
            (double point, double decisionRuleValue) valueOfMinimumErrorPoint)
        {
            int lastPos = 0;

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].point > valueOfMinimumErrorPoint.point)
                {
                    lastPos = i - 1;
                }
            }

            (double point, double decisionRuleValue)[] chart = 
                new (double point, double decisionRuleValue)[lastPos + 1];

            Array.Copy(values, chart, lastPos);
            chart[lastPos] = valueOfMinimumErrorPoint;

            return GetArea(chart);
        }

        private double CalculateDetectionSkipError(
            (double point, double decisionRuleValue)[] values,
            (double point, double decisionRuleValue) valueOfMinimumErrorPoint)
        {
            int firstPos = 0;

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].point > valueOfMinimumErrorPoint.point)
                {
                    firstPos = i;
                }
            }

            (double point, double decisionRuleValue)[] chart = 
                new (double point, double decisionRuleValue)[values.Length - firstPos + 1];

            Array.Copy(values, firstPos, chart, 1, chart.Length - 1);
            chart[0] = valueOfMinimumErrorPoint;

            return GetArea(chart);
        }

        /// <summary>
        /// Calculates area under the chart   
        /// </summary>
        private double GetArea((double point, double decisionRuleValue)[] values)
        {
            if (values.Length <= 1)
            {
                throw new ApplicationException("Chart should contain 2 or more points");
            }

            double area = 0;

            for (int i = 1; i < values.Length; i++)
            {
                area += GetTrapeziumArea(values[i].point - values[i - 1].point, 
                    values[i - 1].decisionRuleValue,
                    values[i].decisionRuleValue);
            }

            return area;
        }

        private double GetTrapeziumArea(double baseLength, double height1, double height2)
        {
            return (height1 + height2) / 2 * baseLength;
        }

        #endregion

        public void Initialize()
        {
            (double leftOfSecond, double rightOfFirst) = GetBounds();

            double[][] sequences = {
                GetSortedSequence(0, rightOfFirst, TotalPoints),
                GetSortedSequence(leftOfSecond, (double)TotalPoints, TotalPoints)
            };

            for (int i = 0; i < data.Length; i++)
            {
                data[i].values = new (double, double)[TotalPoints];

                for (int j = 0; j < sequences[i].Length; j++)
                {
                    data[i].values[j].point = sequences[i][j];
                }

                data[i].mean = GetMean(sequences[i]);
                data[i].standardDeviation = GetStandardDeviation(sequences[i], data[i].mean);
            }

            IsInitialized = true;
            IsCalculated = false;
        }

        /// <summary>
        /// Calculates FalseAlarmError, DetectionSkipError, ValueOfMinimumErrorPoint, 
        /// DecisionRuleValues (data[0].decisionRuleValues & data[1].decisionRuleValues)       
        /// </summary>
        /// <param name="priorProbability">Prior probability that point belongs to first sequence</param>
        public void CalculateData(double priorProbability)
        {
            if (!IsInitialized)
            {
                throw new ApplicationException("Not initialzed");
            }

            double[][] sequences = {
                data[0].values.Select(i => i.decisionRuleValue).ToArray(),
                data[1].values.Select(i => i.decisionRuleValue).ToArray()
            };

            double[][] decisionRuleValues = {
                CalculateDecisionRuleValuesSequence(sequences[0], priorProbability,
                    data[0].mean, data[0].standardDeviation),
                CalculateDecisionRuleValuesSequence(sequences[1], priorProbability,
                    data[1].mean, data[1].standardDeviation),
            };

            // set decision rule values
            for (int i = 0; i < data.Length; i++)
            {
                for (int j = 0; j < data[i].values.Length; j++)
                {
                    data[i].values[j].decisionRuleValue = decisionRuleValues[i][j];
                }
            }
                
            ValueOfMinimumErrorPoint = CalculateValueOfMinimumErrorPoint(data[0].values, data[1].values);
            FalseAlarmError = CalculateFalseAlarmError(data[0].values, ValueOfMinimumErrorPoint);
            DetectionSkipError = CalculateDetectionSkipError(data[1].values, valueOfMinimumErrorPoint);

            IsCalculated = true;
        }
    }
}





