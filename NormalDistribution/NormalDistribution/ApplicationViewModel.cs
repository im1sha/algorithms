using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace NormalDistribution
{
    public class ApplicationViewModel : INotifyPropertyChanged 
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
                OnPropertyChanged();
            }
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
                        //Model = new ApplicationModel(int.Parse(InteractData.TotalPoints), 
                        //    InteractData.DEFAULT_IMAGE_SIZE_IN_PIXELS);
                        //Model.StartExecution();                       
                    }));
            }
        }

       
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }      
    }
}
