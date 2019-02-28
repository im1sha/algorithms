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

namespace Maximin
{
    public class ApplicationModel : IDisposable
    {
        public bool IsCompleted { get; private set; }

        private readonly uint[] colors;
        public BitmapSource Image { get; private set; }

        public int Iteration { get; private set; }
        public long TimeInMs { get; private set; }

        private Maximin executionLogic;

        private CancellationToken token;
        private CancellationTokenSource tokenSource;
        private Task task;

        public ApplicationModel(int clusters, int points, int size, uint[] colors)
        {
            this.colors = colors;
            executionLogic = new Maximin(clusters, points, size);
        }

        /// <summary>
        /// Starts execution in standard threadpool 
        /// </summary>
        public void StartExecution()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            task = Task.Run((Action)Execute, token);          
        }

        private void SetImage(BitmapSource newValue)
        {
            newValue.Freeze();
            Image = newValue;
        }

        private void Execute()
        {
            executionLogic.Initialize();

            (StaticPoint Сenter, StaticPoint[] StaticPoints)[] currentClusterizaiton;

            int currentIteration = 0;
            var chrono = new Stopwatch();
            chrono.Start();        
            do
            {               
                token.ThrowIfCancellationRequested();

                currentClusterizaiton = executionLogic.Reclusterize();

                // retrieve data for UI
                if (currentClusterizaiton != null)
                {
                    BitmapSource image = DataToBitmapConverter.ClustersToBitmap(currentClusterizaiton,
                        executionLogic.MaxCoordinate, executionLogic.MaxCoordinate, colors);
                    SetImage(image);

                    Iteration = ++currentIteration;
                    TimeInMs = chrono.ElapsedMilliseconds;
                }
            } while (!executionLogic.IsFinalState);

            chrono.Stop();
            IsCompleted = true;
        }
    

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) { }

                try
                {
                    // cancel running task
                    if (!task.IsCompleted)
                    {
                        try
                        {
                            tokenSource.Cancel();
                            try
                            {
                                task.Wait();
                            }
                            finally
                            {
                                tokenSource.Dispose();
                            }
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
        #endregion
    }
}


