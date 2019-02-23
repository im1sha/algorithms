using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kmeans
{
    public class ApplicationModel 
    {
        public BitmapSource Image { get; private set; }

        private Algorithm executionLogic;

        private readonly uint[] colors;

        public ApplicationModel(int clusters, int points, int size, uint[] colors)
        {
            this.colors = colors;
            executionLogic = new Algorithm(clusters, points, size);
        }

        public void StartExecution()
        {
            Task.Run((Action)Execute);          
        }

        private void SetImage(BitmapSource newValue)
        {
            newValue.Freeze();
            Image = newValue;
        }

        private void Execute()
        {
            executionLogic.SetInitialClustarization();

            (StaticPoint Сenter, StaticPoint[] StaticPoints)[] currentClusterizaiton;

            do
            {
                currentClusterizaiton = executionLogic.Reclusterize();

                if (currentClusterizaiton != null)
                {
                    BitmapSource image = DataToBitmapConverter.ClustersToBitmap(currentClusterizaiton,
                        executionLogic.MaxCoordinate, executionLogic.MaxCoordinate, colors);
                    SetImage(image);
                }
            } while (!executionLogic.IsFinalState);
        }
    }
}


