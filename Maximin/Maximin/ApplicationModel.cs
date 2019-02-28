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

        public BitmapSource Image { get; private set; }

        public int Clusters { get; private set; } = 0;
        public long TimeInMs { get; private set; } = 0;

        private Maximin executionLogic;

        private CancellationToken token;
        private CancellationTokenSource tokenSource;
        private Task task;

        public ApplicationModel(int points, int size)
        {
            executionLogic = new Maximin(points, size);
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
            List<uint> colors = new List<uint>();

            ColorGenerator colorGenerator = new ColorGenerator();

            executionLogic.Initialize();

            Clusters++;
            colors.Add(colorGenerator.NextColorAsUInt());

            (StaticPoint Сenter, StaticPoint[] StaticPoints)[] currentClusterizaiton;

            var chrono = new Stopwatch();
            chrono.Start();        
            do
            {               
                token.ThrowIfCancellationRequested();

                currentClusterizaiton = executionLogic.Reclusterize();

                // retrieve data for UI
                if (currentClusterizaiton != null)
                {
                    Clusters++;
                    colors.Add(colorGenerator.NextColorAsUInt());

                    BitmapSource image = DataToBitmapConverter.ClustersToBitmap(currentClusterizaiton,
                        executionLogic.MaxCoordinate, executionLogic.MaxCoordinate, colors.ToArray());
                    SetImage(image);
                    
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


