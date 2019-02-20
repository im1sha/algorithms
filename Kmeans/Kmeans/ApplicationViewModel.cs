using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Kmeans
{
    public class ApplicationViewModel /*: INotifyPropertyChanged*/
    {
        public InitialData InitialData { get; set; } = new InitialData();

        private ApplicationModel model;

        //public ApplicationModel Model
        //{
        //    get { return model; }
        //    set { model = value; OnPropertyChanged("Model"); }
        //}

        // command of execution starting  
        private InteractCommand executeCommand;
        public InteractCommand ExecuteCommand
        {
            get
            {
                return executeCommand ??
                    (executeCommand = new InteractCommand(obj =>
                    {
                        model = new ApplicationModel(InitialData.TotalClusters, 
                            InitialData.TotalPoints, InitialData.DefaultImageSizeInPixels, 
                            InitialData.Colors);
                        model.StartExecution();
                    }));
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //public void OnPropertyChanged([CallerMemberName]string property = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        //}
    }
}




