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

namespace NormalDistribution
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel();
            CompositionTarget.Rendering += OnRendering;
        }

        void OnRendering(object sender, EventArgs e)
        {
            if (DataContext is IRefresh)
                ((IRefresh)DataContext).Refresh();
        }

        //#region Test Canvas

        //Point currentPoint = new Point();

        //private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ButtonState == MouseButtonState.Pressed)
        //    {
        //        currentPoint = e.GetPosition(this);
        //    }
        //}

        //private void Canvas_MouseMove_1(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        Line line = new Line
        //        {
        //            Stroke = SystemColors.WindowFrameBrush,
        //            X1 = currentPoint.X,
        //            Y1 = currentPoint.Y,
        //            X2 = e.GetPosition(this).X,
        //            Y2 = e.GetPosition(this).Y
        //        };

        //        currentPoint = e.GetPosition(this);

        //        paintSurface.Children.Add(line);
        //    }
        //}

        //#endregion

    }
}
