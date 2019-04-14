using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NormalDistribution
{
    public class ApplicationModel : IDisposable
    {
        private readonly InteractData interactData;

        private Algorithm algorithm;

        #region Tasks cancellation

        private CancellationTokenSource initializationTokenSource;
        private Task initializationTask;

        private CancellationTokenSource calculationTokenSource;
        private Task calculationTask;

        #endregion

        public ApplicationModel(InteractData interactData)
        {
            this.interactData = interactData ?? throw new ArgumentNullException();

            algorithm = new Algorithm(int.Parse(this.interactData.TotalPoints),
                InteractData.DEFAULT_IMAGE_SIZE_IN_PIXELS);
        }

        public void StartInitialization()
        {
            initializationTokenSource = new CancellationTokenSource();
            initializationTask = Task.Run((Action)Initialize, initializationTokenSource.Token);
        }

        private void Initialize()
        {
            algorithm.Initialize();
            interactData.IsInitialized = true;
        }

        public void StartDataProcessing()
        {
            if (interactData.IsInitialized)
            {
                if (calculationTokenSource != null && calculationTask != null)
                {
                    WaitForTaskAndFreeResources(calculationTask, calculationTokenSource);
                }

                calculationTokenSource = new CancellationTokenSource();

                calculationTask = Task.Run(() =>
                    ProcessData(int.Parse(interactData.Probability) / 100.0f),
                    calculationTokenSource.Token);
            };
        }

        private void ProcessData(float probability)
        {
            algorithm.CalculateData(probability);

            interactData.SummaryError = algorithm.Error;
            interactData.DetectionSkipError = algorithm.DetectionSkipError;
            interactData.FalseAlarmError = algorithm.FalseAlarmError;

            (float point, float decisionRuleValue) valueOfMinimumErrorPoint = algorithm.ValueOfMinimumErrorPoint;
            (float point, float decisionRuleValue)[][] values = algorithm.Values;

            //
            float multiplierOfDecisionRuleValues = InteractData.DEFAULT_IMAGE_SIZE_IN_PIXELS /
                Math.Max(values[0].Max(i => i.decisionRuleValue), values[1].Max(i => i.decisionRuleValue));

            // flip uside down
            var decisionRuleValues0 = values[0].Select(i => (int)(InteractData.DEFAULT_IMAGE_SIZE_IN_PIXELS - i.decisionRuleValue * multiplierOfDecisionRuleValues)).ToArray();
            var decisionRuleValues1 = values[1].Select(i => (int)(InteractData.DEFAULT_IMAGE_SIZE_IN_PIXELS - i.decisionRuleValue * multiplierOfDecisionRuleValues)).ToArray();

            interactData.SetImage(
                ImageGenerator.GetImageByData(
                    MergeArrays(values[0].Select(i => (int)i.point).ToArray(),  decisionRuleValues0),
                    MergeArrays(values[1].Select(i => (int)i.point).ToArray(),  decisionRuleValues1),
                    (int)valueOfMinimumErrorPoint.point,
                    InteractData.DEFAULT_IMAGE_SIZE_IN_PIXELS));
        }

        private int[] MergeArrays(int[] source1, int[] source2)
        {
            if (source1.Length != source2.Length)
            {
                throw new ArgumentException();
            }

            int[] result = new int[source1.Length * 2];

            for (int i = 0; i < source1.Length; i ++)
            {
                result[i * 2] = source1[i];
                result[i * 2 + 1] = source2[i];
            }

            return result;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                //if (disposing) { }

                try
                {
                    // cancel running tasks
                    WaitForTaskAndFreeResources(initializationTask, initializationTokenSource);
                    WaitForTaskAndFreeResources(calculationTask, calculationTokenSource);
                }
                finally
                {
                    disposedValue = true;
                }
            }
        }

        ~ApplicationModel()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void WaitForTaskAndFreeResources(Task task, CancellationTokenSource tokenSource)
        {
            try
            {
                if (task != null && !task.IsCompleted)
                {
                    try
                    {
                        tokenSource.Cancel();                       
                        task.Wait();                       
                    }
                    catch (AggregateException ae)
                    {
                        foreach (var e in ae.Flatten().InnerExceptions)
                        {
                            if (!(e is OperationCanceledException))
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            finally
            {
                tokenSource?.Dispose();
            }
        }

        #endregion
    }
}


