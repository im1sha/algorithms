using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Kmeans
{
    public class ApplicationViewModel : INotifyPropertyChanged, IRefresh
    {
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


        public void Refresh()
        {
            if (model == null)
            {
                return;
            }
            Image = model.Image;
        }

        // command of execution starting  
        private InteractCommand executeCommand;
        public InteractCommand ExecuteCommand
        {
            get
            {
                return executeCommand ??
                    (executeCommand = new InteractCommand(obj =>
                    {
                        Model = new ApplicationModel(InitialData.TotalClusters, InitialData.TotalPoints, 
                            InitialData.DefaultImageSizeInPixels, InitialData.Colors);
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




