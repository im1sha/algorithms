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
        public readonly int TotalPoints;

        /// <summary>
        /// decisionRuleValues is ( prior_probability * posterior_probability ), 
        /// mean - матожидание,  
        /// standardDeviation - среднее кв отклонение  
        /// </summary>
        private ((float point, float decisionRuleValue)[] values, float mean, float standardDeviation)[] data
            = new ((float point, float decisionRuleValue)[] values, float mean, float standardDeviation)[2];

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
            TotalPoints = totalPoints;
        }

        #region Initialization

        private (float leftOfSecond, float rightOfFirst) GetBounds()
        {
            Random random = new Random();

            float leftOfSecond = random.Next(0, MaxPointPosition / 2);
            float rightOfFirst = random.Next(MaxPointPosition / 2, MaxPointPosition);

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
            return (float)(Math.Exp(-0.5 * Math.Pow((x - mean) / standardDeviation, 2))
                 / (standardDeviation * Math.Sqrt(2 * Math.PI)));
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
            var result = (0f, 0f);

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
            float falseAlarmErrorArea = CalculateFalseAlarmErrorArea(values1, valueOfMinimumErrorPoint);
            float detectionSkipErrorArea = CalculateDetectionSkipErrorArea(values2, valueOfMinimumErrorPoint);
            float chart1Area = GetArea(values1);
            float chart2Area = GetArea(values2);
            float fullArea = chart1Area + chart2Area - falseAlarmErrorArea - detectionSkipErrorArea;

            return (falseAlarmErrorArea / fullArea, detectionSkipErrorArea / fullArea); // <?
        }

        private float CalculateFalseAlarmErrorArea(
           (float point, float decisionRuleValue)[] values,
           (float point, float decisionRuleValue) valueOfMinimumErrorPoint)
        {
            int lastPos = 0;

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].point > valueOfMinimumErrorPoint.point)
                {
                    lastPos = i - 1;
                    break;
                }
            }

            (float point, float decisionRuleValue)[] chart =
                new (float point, float decisionRuleValue)[lastPos + 1 + 1];

            Array.Copy(values, chart, lastPos + 1);
            chart[lastPos + 1] = valueOfMinimumErrorPoint;

            return GetArea(chart);
        }

        private float CalculateDetectionSkipErrorArea(
            (float point, float decisionRuleValue)[] values,
            (float point, float decisionRuleValue) valueOfMinimumErrorPoint)
        {
            int firstPos = 0;

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i].point > valueOfMinimumErrorPoint.point)
                {
                    firstPos = i;
                    break;
                }
            }

            (float point, float decisionRuleValue)[] chart =
                new (float point, float decisionRuleValue)[values.Length - firstPos + 1];

            Array.Copy(values, firstPos, chart, 1, chart.Length - 1);
            chart[0] = valueOfMinimumErrorPoint;

            return GetArea(chart);
        }

        /// <summary>
        /// Calculates area under the chart   
        /// </summary>
        private float GetArea((float point, float decisionRuleValue)[] values)
        {
            if (values.Length <= 1)
            {
                throw new ApplicationException("Chart should contain 2 or more points");
            }

            float area = 0;

            for (int i = 1; i < values.Length; i++)
            {
                area += GetTrapeziumArea(values[i].point - values[i - 1].point,
                    values[i - 1].decisionRuleValue,
                    values[i].decisionRuleValue);
            }

            return area;
        }

        private float GetTrapeziumArea(float baseLength, float height1, float height2)
        {
            return (height1 + height2) / 2 * baseLength;
        }

        #endregion

        public void Initialize()
        {
            (float leftOfSecond, float rightOfFirst) = GetBounds();

            float[][] sequences = {
                GetSortedSequence(0, rightOfFirst, TotalPoints),
                GetSortedSequence(leftOfSecond, (float)MaxPointPosition, TotalPoints)
            };

            for (int i = 0; i < data.Length; i++)
            {
                data[i].values = new (float, float)[TotalPoints];

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
                CalculateDecisionRuleValuesSequence(sequences[1], priorProbability,
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

            Log();

            IsCalculated = true;
        }

        private void Log()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"falsealarmerror: {falseAlarmError}");
            sb.AppendLine($"detectionskiperror: {detectionSkipError}\n");
            sb.AppendLine($"valueofminimumerrorpoint: POINT {valueOfMinimumErrorPoint.point} DECISIONRULEVALUE {valueOfMinimumErrorPoint.decisionRuleValue}\n");

            sb.AppendLine($"data[0] MEAN {data[0].mean} data[0] STANDARDDEVIATION { data[0].standardDeviation}\n");

            sb.AppendLine($"data[0] LEFT " +
                $"POINT {data[0].values[0].point} " +
                $"DECISIONRULEVALUE {data[0].values[0].decisionRuleValue} \n" +
                $"data[0] RIGHT " +
                $"POINT {data[0].values[data[0].values.Length - 1].point} " +
                $"DECISIONRULEVALUE {data[0].values[data[0].values.Length - 1].decisionRuleValue}\n");

            sb.AppendLine($"data[1] MEAN {data[1].mean} data[1] STANDARDDEVIATION { data[1].standardDeviation}\n");

            sb.AppendLine($"data[1] LEFT " +
              $"POINT {data[1].values[1].point} " +
              $"DECISIONRULEVALUE {data[1].values[1].decisionRuleValue} \n" +
              $"data[1] RIGHT " +
              $"POINT {data[1].values[data[1].values.Length - 1].point} " +
              $"DECISIONRULEVALUE {data[1].values[data[1].values.Length - 1].decisionRuleValue}\n");

            for (int i = 0; i < data.Length; i++)
            {
                sb.AppendLine($"\n\n DATA {i} ===============================================================\n\n");
                for (int j = 0; j < data[i].values.Length; j++)
                {
                    sb.AppendLine($"POINT {data[i].values[j].point} DECISIONRULEVALUE {data[i].values[j].decisionRuleValue }");
                }
            }

            //for (int i = 0; i < _loggedpoints.Count; i++)
            //{
            //    sb.AppendLine($"{_loggedpoints[i].Item1.Item1} : {_loggedpoints[i].Item1.Item2} | {_loggedpoints[i].Item2.Item1} : {_loggedpoints[i].Item2.Item2}");
            //}

            File.WriteAllText("log" + DateTime.Now.ToBinary().ToString() + ".txt", sb.ToString());
        }

        //List<((double, double),(double,double))> _loggedpoints;
    }
}




