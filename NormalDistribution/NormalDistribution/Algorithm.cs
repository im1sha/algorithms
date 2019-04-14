using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace NormalDistribution
{
    class Algorithm
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsCalculated { get; private set; } = false;

        public readonly int MaxPointPosition;
        private readonly int InitializationSetSize;

        /// <summary>
        /// decisionRuleValues is ( prior_probability * posterior_probability ), 
        /// mean - матожидание,  
        /// standardDeviation - среднее кв отклонение  
        /// </summary>
        private ((float point, float decisionRuleValue)[] values, float mean, float standardDeviation)[] data = 
            new ((float point, float decisionRuleValue)[] values, float mean, float standardDeviation)[2];

        #region Calculactions results

        private float falseAlarmError; // вероятность ложной тревоги
        public float FalseAlarmError {
            get { ThrowIfNotCalculated(); return falseAlarmError; }
            private set { falseAlarmError = value; }
        }

        private float detectionSkipError; // вероятность пропуска ошибки
        public float DetectionSkipError {
            get { ThrowIfNotCalculated(); return detectionSkipError; }
            private set { detectionSkipError = value; }
        }

        public float Error { get { ThrowIfNotCalculated(); return DetectionSkipError + FalseAlarmError; } }

        private (float point, float decisionRuleValue) valueOfMinimumErrorPoint; // intersection of decisionRuleValues[0] and decisionRuleValues[1]
        public (float point, float decisionRuleValue) ValueOfMinimumErrorPoint {
            get { ThrowIfNotCalculated(); return valueOfMinimumErrorPoint; }
            private set { valueOfMinimumErrorPoint = value; }
        }

        public (float point, float decisionRuleValue)[][] Values
        {
            get {
                ThrowIfNotCalculated();
                return new (float point, float decisionRuleValue)[][] {
                   ((float point, float decisionRuleValue)[]) data[0].values.Clone(),
                   ((float point, float decisionRuleValue)[]) data[1].values.Clone()
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
            InitializationSetSize = totalPoints;
        }

        #region Initialization

        private (float leftOfSecond, float rightOfFirst) GetBounds()
        {
            Random random = new Random();

            float leftOfSecond = random.Next(0, MaxPointPosition / 3);
            float rightOfFirst = MaxPointPosition - random.Next(0, MaxPointPosition / 3); 

            return (leftOfSecond, rightOfFirst);
        }

        private float[] GetSortedSequence(float leftBound, float rightBound, int totalPoints)
        {
            float interval = rightBound - leftBound;

            Random r = new Random();
            var result = new float[totalPoints];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (float)r.NextDouble() * interval + leftBound;
            }
            Array.Sort(result);
            return result;
        }

        private float GetMean(float[] sequence)
        {
            return sequence.Sum() / sequence.Length;
        }

        private float GetStandardDeviation(float[] sequence, float mean)
        {
            return (float) Math.Sqrt(sequence.Select(x => Math.Pow(x - mean, 2)).Sum() / sequence.Length);
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates posterior probability for normal distribution
        /// </summary>
        private float GetPosteriorProbability(float x, float mean, float standardDeviation)
        {
            return (float)(Math.Exp(-0.5 * Math.Pow((x - mean) / standardDeviation, 2)) /
                (standardDeviation * Math.Sqrt(2 * Math.PI)));
        }

        /// <summary>
        /// Values used to determine class which point belongs
        /// </summary>
        private float GetDecisionRuleValue(float priorProbability, float posteriorProbability)
        {
            return priorProbability * posteriorProbability;
        }

        /// <summary>
        /// Calculate DecisionRuleValue for each item of the sequence
        /// </summary>
        private float[] CalculateDecisionRuleValuesSequence(float[] sequence, float priorProbability,
            float mean, float standardDeviation)
        {
            return sequence.Select(x => GetDecisionRuleValue(priorProbability, GetPosteriorProbability(x, mean, standardDeviation))).ToArray();
        }
   
        private readonly object sync = new object();

        private (float point, float decisionRuleValue) CalculateValueOfMinimumErrorPoint(
            (float point, float decisionRuleValue)[] values1,
            (float point, float decisionRuleValue)[] values2)
        {
            var result = (float.NaN, float.NaN);

            Parallel.For(1, values1.Length, (i) =>
            {
                Parallel.For(1, values2.Length, (j) =>
                {
                    (float X, float Y) = Point.FindIntersection(values1[i-1], values1[i], values2[j-1], values2[j]);  
                    
                    if (!float.IsNaN(X) && !float.IsNaN(Y))
                    {
                        lock (sync)
                        {
                            result = (X, Y);
                        }
                    }
                });
            });

            return result;
        }

        private (float falseAlarmError, float detectionSkipError) CalculateErrors(
            (float point, float decisionRuleValue)[] values1,
            (float point, float decisionRuleValue)[] values2,
            (float point, float decisionRuleValue) valueOfMinimumErrorPoint
            )
        {
            if (float.IsNaN(valueOfMinimumErrorPoint.point) && float.IsNaN(valueOfMinimumErrorPoint.decisionRuleValue))
            {
                if (values1[0].decisionRuleValue > values2[0].decisionRuleValue)
                {
                    return (values2.Select(i => i.decisionRuleValue).Sum(), 0);
                }
                else
                {
                    return (0, values1.Select(i => i.decisionRuleValue).Sum());
                }
            }
            else
            {
                float falseAlarmErrorArea = values2.Where(i => i.point < valueOfMinimumErrorPoint.point).Select(i => i.decisionRuleValue).Sum();
                float detectionSkipErrorArea = values1.Where(i => i.point > valueOfMinimumErrorPoint.point).Select(i => i.decisionRuleValue).Sum();

                float fullArea = values1.Select(i => i.decisionRuleValue).Sum() +
                    values2.Select(i => i.decisionRuleValue).Sum() -
                    falseAlarmErrorArea - detectionSkipErrorArea;

                return (falseAlarmErrorArea / fullArea, detectionSkipErrorArea / fullArea);
            }
        }

        #endregion

        public void Initialize()
        {
            (float leftOfSecond, float rightOfFirst) = GetBounds();

            float[][] initializationSequences = {
                GetSortedSequence(0, rightOfFirst, InitializationSetSize),
                GetSortedSequence(leftOfSecond, (float)MaxPointPosition, InitializationSetSize)
            };

            for (int i = 0; i < data.Length; i++)
            {
                data[i].mean = GetMean(initializationSequences[i]);
                data[i].standardDeviation = GetStandardDeviation(initializationSequences[i], data[i].mean);
                data[i].values = new (float, float)[MaxPointPosition];

                for (int j = 0; j < data[i].values.Length; j++)
                {
                    data[i].values[j].point = j;
                }             
            }

            IsInitialized = true;
            IsCalculated = false;
        }

        /// <summary>
        /// Calculates FalseAlarmError, DetectionSkipError, ValueOfMinimumErrorPoint, 
        /// DecisionRuleValues (data[0].decisionRuleValues & data[1].decisionRuleValues)       
        /// </summary>
        /// <param name="priorProbability">Prior probability that point belongs to first sequence</param>
        public void CalculateData(float priorProbability)
        {
            if (!IsInitialized)
            {
                throw new ApplicationException("Not initialzed");
            }

            float[][] sequences = {
                data[0].values.Select(i => i.point).ToArray(),
                data[1].values.Select(i => i.point).ToArray()
            };

            float[][] decisionRuleValues = {
                CalculateDecisionRuleValuesSequence(sequences[0], priorProbability,
                    data[0].mean, data[0].standardDeviation),
                CalculateDecisionRuleValuesSequence(sequences[1], 1 - priorProbability,
                    data[1].mean, data[1].standardDeviation),
            };

            Parallel.For(0, data.Length, i =>
            {
                Parallel.For(0, data[i].values.Length, j =>
                {
                    data[i].values[j].decisionRuleValue = decisionRuleValues[i][j];
                });
            });
            
            valueOfMinimumErrorPoint = CalculateValueOfMinimumErrorPoint(data[0].values, data[1].values);
            (falseAlarmError, detectionSkipError) = CalculateErrors(data[0].values, data[1].values, valueOfMinimumErrorPoint);

            IsCalculated = true;
        }   
    }
}




