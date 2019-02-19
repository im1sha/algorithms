using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Kmeans
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public InitialData InitialData { get; set; } = new InitialData();

        private bool isStarted = false;
        public bool IsStarted
        {
            get { return isStarted; }
            set
            {
                isStarted = value;
                OnPropertyChanged("IsStarted");
            }
        }

        private ApplicationModel model;
        public ApplicationModel Model
        {
            get { return model; }
            set
            {
                model = value;
                OnPropertyChanged("Model");
            }
        }

        public ApplicationViewModel()
        {
            Model = new ApplicationModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

    }
}
