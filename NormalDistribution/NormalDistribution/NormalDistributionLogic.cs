using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NormalDistribution
{
    class NormalDistributionLogic
    {
        public bool IsInitialized { get; private set; } = false;
        public bool IsCalculated { get; private set; } = false;

        private readonly int maxPointPosition;
        private readonly int totalPoints;

        private (double[] points, double[] decisionRuleValues, double mean, double standardDeviation)[] data 
            = new (double[] points, double[] decisionRuleValues, double mean, double standardDeviation)[2];

        private double falseAlarmError; 
        private double detectionSkipError;
        private double minimumErrorLineValue;
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalPoints">Single sequence length</param>
        /// <param name="maxPointPosition"></param>
        public NormalDistributionLogic(int totalPoints, int maxPointPosition)
        {
            this.maxPointPosition = maxPointPosition;
            this.totalPoints = totalPoints;
        }

        private (double leftOfSecond, double rightOfFirst) GetBounds()
        {
            Random random = new Random();

            double leftOfSecond = random.Next(0, maxPointPosition / 2);
            double rightOfFirst = random.Next(maxPointPosition / 2, maxPointPosition);
            
            return (leftOfSecond, rightOfFirst);
        }

        private double[] GetSortedSequence(double leftBound, double rightBound)
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
           return Math.Sqrt(sequence.Select(i => Math.Pow(i - mean, 2)).Sum() / sequence.Length);
        }

        private double GetPosteriorProbability(double x, double mean, double standardDeviation)
        {
           return Math.Exp(-0.5 * Math.Pow((x - mean) / standardDeviation, 2)) 
                / (standardDeviation * Math.Sqrt(2 * Math.PI));
        }

        private double GetDecisionRuleValue(double priorProbability, double posteriorProbability)
        {
            return priorProbability * posteriorProbability;
        }

        // private double Calculate

        public void Initialize()
        {
            (double leftOfSecond, double rightOfFirst) = GetBounds();

            data[0].points = GetSortedSequence(0, rightOfFirst);
            data[1].points = GetSortedSequence(leftOfSecond, totalPoints);

            for (int i = 0; i < data.Length; i++)
            {
                data[i].decisionRuleValues = new double[data[i].points.Length];
                data[i].mean = GetMean(data[i].points);
                data[i].standardDeviation = GetStandardDeviation(data[i].points, data[i].mean);
            }

            IsInitialized = true;
            IsCalculated = false;
        }

        public double GetFalseAlarmError() { return falseAlarmError; }
        public double GetDetectionSkipError() { return detectionSkipError; }
        public double GetMinimumErrorLineValue() { return minimumErrorLineValue; }
        public double[][] GetDecisionRuleValues()
        {
            return new double[2][] {
                (double[]) data[0].decisionRuleValues.Clone(),
                (double[]) data[1].decisionRuleValues.Clone()
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priorProbability">Prior probability that point belongs to first sequence</param>
        public void CalculateData(double priorProbability)
        {
            if (!IsInitialized)
            {
                throw new ApplicationException();
            }

            // private (double[] points, double[] decisionRuleValues, double mean, double standardDeviation)[] data; 

            for (int i = 0; i < data.Length; i++)
            {         
                for (int j = 0; j < data[i].points.Length; j++)
                {
                   // decisionRuleValues[j] = GetDecisionRuleValue();
                }
            }
        }
    }
}



