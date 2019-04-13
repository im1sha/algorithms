using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
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
            get => model;            
            set
            {
                if (value != model)
                {
                    model = value;
                    OnPropertyChanged();
                }              
            }
        }

        private InteractCommand generateCommand;
        public InteractCommand GenerateCommand
        {
            get
            {
                return generateCommand ??
                    (generateCommand = new InteractCommand(obj =>
                    {
                        Model?.Dispose();

                        Model = new ApplicationModel(InteractData);

                        Model.StartInitialization();
                    }));
            }
        }

        private InteractCommand processCommand;
        public InteractCommand ProcessCommand
        {
            get
            {
                return processCommand ??
                    (processCommand = new InteractCommand(obj =>
                    {
                        if (Model != null && InteractData.IsInitialized)
                        {
                            Model.StartDataProcessing();
                        }
                    }));
            }
        }

        #region INotifyPropertyChanged 

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
