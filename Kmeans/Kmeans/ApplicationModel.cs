using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Kmeans
{
    public class ApplicationModel : INotifyPropertyChanged
    {
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

        private Algorithm executionLogic;

        public ApplicationModel(int clusters, int points, int size)
        {
            executionLogic = new Algorithm(clusters, points, size);
        }

        public void StartExecution()
        {
            // Point's positions & cluster's centers generating 
            executionLogic.SetInitialClustarization();

            //do {
            ////	  формирование кластеров вокруг центров	
            ////	  расчет центров
            //} while ();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string property = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}


