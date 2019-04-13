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

        #region Algorithm data

        private Algorithm algorithm;

        private (double point, double decisionRuleValue) valueOfMinimumErrorPoint;
        public (double point, double decisionRuleValue)[][] values;

        #endregion

        #region Task cancellation

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
                    ProcessData(int.Parse(interactData.Probability) / 100.0D),
                    calculationTokenSource.Token);
            };
        }

        private void ProcessData(double probability)
        {
            algorithm.CalculateData(probability);

            interactData.SummaryError = algorithm.Error;
            interactData.DetectionSkipError = algorithm.DetectionSkipError;
            interactData.FalseAlarmError = algorithm.FalseAlarmError;


            values = algorithm.Values;
            valueOfMinimumErrorPoint = algorithm.ValueOfMinimumErrorPoint;


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
                if (!task.IsCompleted)
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
                tokenSource.Dispose();
            }
        }

        #endregion
    }
}


