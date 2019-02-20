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
    public class ApplicationViewModel
    {
        public InitialData InitialData { get; set; } = new InitialData();

        private ApplicationModel model;

        // command of execution starting  
        private InteractCommand executeCommand;
        public InteractCommand ExecuteCommand
        {
            get
            {
                return executeCommand ??
                    (executeCommand = new InteractCommand(obj =>
                    {
                        model = new ApplicationModel(InitialData.TotalClusters, InitialData.TotalPoints,
                            InitialData.DefaultImageSizeInPixels);
                        model.StartExecution();
                    }));
            }
        }
    }
}




