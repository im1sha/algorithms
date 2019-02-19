using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.ComponentModel;

namespace Kmeans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            

            var applicationViewModel  = new ApplicationViewModel();

            mainGrid.DataContext = applicationViewModel;
            inputGrid.DataContext = applicationViewModel?.InitialData;
            
            //image.Source = random();
        }



        //private async void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    txt.Text = "started";
        //    await Task.Run(() => HeavyMethod(this));
        //    txt.Text = "done";
        //}
        //internal void HeavyMethod(MainWindow gui)
        //{
        //    while (stillWorking)
        //    {
        //        window.Dispatcher.Invoke(() =>
        //        {
        //            // UI operations go inside of Invoke
        //            txt.Text += ".";
        //        });

        //        // Heavy operations go outside of Invoke
        //        System.Threading.Thread.Sleep(51);
        //    }
        //}


    }
}
