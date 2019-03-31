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
        private bool IsMaximinStarted { get; set; } = false;
        private bool IsMaximinCompleted { get; set; } = false;
        private bool IsKmeansApplied { get; set; } = false;
        private bool IsKmeansCompleted { get; set; } = false;

        public bool IsStarted { get { return IsMaximinStarted; } }
        public bool CanApplyKmeans { get { return IsMaximinCompleted && !IsKmeansApplied; } }
        public bool CanApplyMaximin { get { return !IsMaximinStarted || IsKmeansCompleted; } }
        public bool IsFinished { get { return IsKmeansCompleted; } }

        public BitmapSource Image { get; private set; }

        public int Clusters { get; private set; } = 0;
        public int Points { get; private set; }
        public int Size { get; private set; }
        public long TimeInMs { get; private set; } = 0;
        public string State { get; private set; } = "Ready.";

        //private MaximinAlgorithm maximin;

        private CancellationToken token;
        private CancellationTokenSource tokenSource;
        private Task task;

        private List<uint> colors;
       // private (StaticPoint Сenter, StaticPoint[] StaticPoints)[] clusterization;
        private Stopwatch timer = new Stopwatch();
      
        public ApplicationModel(int points, int size)
        {
            Size = size;
            Points = points;
       //     maximin = new MaximinAlgorithm(points, size);
        }

        /// <summary>
        /// Starts execution in standard threadpool 
        /// </summary>
        public void StartExecution()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            task = Task.Run((Action)ExecuteMaximin, token);          
        }

        private void ExecuteMaximin()
        {
            //IsMaximinStarted = true;
            //State = "Maximin is running.";

            //colors = new List<uint>();
            //ColorGenerator colorGenerator = new ColorGenerator();
            //(StaticPoint Сenter, StaticPoint[] StaticPoints)[] currentClusterizaiton;

            //maximin.Initialize();
            //Clusters++;
            //colors.Add(colorGenerator.NextColorAsUInt());
       
            //timer.Start();        
            //do
            //{               
            //    token.ThrowIfCancellationRequested();

            //    currentClusterizaiton = maximin.Reclusterize();
            //    // retrieve data for UI
            //    if (currentClusterizaiton != null)
            //    {
            //        clusterization = currentClusterizaiton;
            //        Clusters++;
            //        colors.Add(colorGenerator.NextColorAsUInt());

            //        BitmapSource image = DataToBitmapConverter.ClustersToBitmap(currentClusterizaiton,
            //            maximin.MaxCoordinate, maximin.MaxCoordinate, colors.ToArray());
            //        SetImage(image);
                    
            //        TimeInMs = timer.ElapsedMilliseconds;
            //    }
            //} while (!maximin.IsFinalState);
            //timer.Stop();

            //State = "Maximin is completed.";
            //IsMaximinCompleted = true;
        }

        public void ApplyKmeans()
        {
            algorithmUseDispose = true;
            Dispose(DisposeRequestType.Inner);

            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            task = Task.Run((Action)ExecuteKmeans, token);

            algorithmUseDispose = false;
            if (isDisposeRequestedByUser)
            {
                Dispose(DisposeRequestType.User);
                isDisposeRequestedByUser = false;
            }
        }

        private void ExecuteKmeans()
        {          
            //if (clusterization == null)
            //{
            //    throw new ApplicationException("Clusterization isn't set.");
            //}

            //IsKmeansApplied = true;
            //Kmeans kmeans = new Kmeans(Clusters, Points, Size);
            //kmeans.SetInitialClustarization(clusterization);

            //(StaticPoint Сenter, StaticPoint[] StaticPoints)[] currentClusterizaiton;
            //int currentIteration = 0;
            //State = $"Kmeans: iteration {currentIteration}.";

            //timer.Start();
            //do
            //{
            //    token.ThrowIfCancellationRequested();

            //    currentClusterizaiton = kmeans.Reclusterize();

            //    // retrieve data for UI
            //    if (currentClusterizaiton != null)
            //    {
            //        BitmapSource image = DataToBitmapConverter.ClustersToBitmap(currentClusterizaiton,
            //            kmeans.MaxCoordinate, kmeans.MaxCoordinate, colors.ToArray());
            //        SetImage(image);

            //        State = $"Kmeans: iteration {++currentIteration}.";
            //        TimeInMs = timer.ElapsedMilliseconds;
            //    }
            //} while (!kmeans.IsFinalState);
            //timer.Stop();

            //State = $"Kmeans is completed.";
            //IsKmeansCompleted = true;
        }

        #region IDisposable Support

        private bool algorithmUseDispose = false; // disposing beetween maximin & kmeans
        private bool isDisposeRequestedByUser = false;

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(DisposeRequestType requst)
        {
            if (algorithmUseDispose && requst == DisposeRequestType.User)
            {
                isDisposeRequestedByUser = true;
                return;
            }

            if (!disposedValue)
            {
                if (requst == DisposeRequestType.GC) { }

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
                    if (requst == DisposeRequestType.GC || requst == DisposeRequestType.User)
                    {
                        disposedValue = true;
                    }
                }
            }
        }

        ~ApplicationModel()
        {
            Dispose(DisposeRequestType.GC);
        }

        public void Dispose()
        {
            Dispose(DisposeRequestType.User);
        }

        protected enum DisposeRequestType { GC, User, Inner/*initiated by algorithm*/ }
        #endregion

        private void SetImage(BitmapSource newValue)
        {
            newValue.Freeze();
            Image = newValue;
        }
    }
}


