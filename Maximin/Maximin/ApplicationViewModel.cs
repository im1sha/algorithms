using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace Maximin
{
    public class ApplicationViewModel : INotifyPropertyChanged, IRefresh
    {
        /// <summary>
        /// User's input and predefined restrictions
        /// </summary>
        public InteractData InteractData { get; set; } = new InteractData();

        private ApplicationModel model;
        public ApplicationModel Model
        {
            get
            {
                return model;
            }
            set
            {
                model = value;
                OnPropertyChanged("Model");
            }
        }

        private BitmapSource image;
        public BitmapSource Image
        {
            get { return image; }
            set
            {
                image = value;
                OnPropertyChanged("Image");
            }
        }

        private int clusters;
        public int Clusters
        {
            get { return clusters; }
            set
            {
                clusters = value;
                OnPropertyChanged("Clusters");
            }
        }

        private long timeInMs;
        public long TimeInMs
        {
            get { return timeInMs; }
            set
            {
                timeInMs = value;
                OnPropertyChanged("TimeInMs");
            }
        }

        private string state;
        public string State
        {
            get { return state; }
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }

        /// <summary>
        /// Interacts with UI
        /// </summary>
        public void Refresh()
        {
            if (model == null)
            {
                return;
            }
            Image = model.Image;
            Clusters = model.Clusters;
            TimeInMs = model.TimeInMs;
            State = model.State;
            InteractData.CanApplyKmeans = model.CanApplyKmeans;
            InteractData.CanApplyMaximin = model.CanApplyMaximin;
        }

        private InteractCommand executeCommand;
        /// <summary>
        /// Starts model execution
        /// </summary>
        public InteractCommand ExecuteCommand
        {
            get
            {
                return executeCommand ??
                    (executeCommand = new InteractCommand(obj =>
                    {
                        Model?.Dispose();                   
                        Model = new ApplicationModel(int.Parse(InteractData.TotalPoints), 
                            InteractData.DEFAULT_IMAGE_SIZE_IN_PIXELS);
                        Model.StartExecution();                       
                    }));
            }
        }

        private InteractCommand applyCommand;
        /// <summary>
        /// Starts model execution
        /// </summary>
        public InteractCommand ApplyCommand
        {
            get
            {
                return applyCommand ??
                    (applyCommand = new InteractCommand(obj =>
                    {
                        Model?.ApplyKmeans();
                    }));
            }
        }

        public ApplicationViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }      
    }
}




