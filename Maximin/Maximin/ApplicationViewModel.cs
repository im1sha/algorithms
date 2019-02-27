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
        public InitialData InitialData { get; set; } = new InitialData();

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

        private int iteration;
        public int Iteration
        {
            get { return iteration; }
            set
            {
                iteration = value;
                OnPropertyChanged("Iteration");
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
            Iteration = model.Iteration;
            TimeInMs = model.TimeInMs;
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
                        Model = new ApplicationModel(int.Parse(InitialData.TotalClusters),
                            int.Parse(InitialData.TotalPoints), InitialData.DEFAULT_IMAGE_SIZE_IN_PIXELS,
                            InitialData.Colors);
                        Model.StartExecution();                       
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




