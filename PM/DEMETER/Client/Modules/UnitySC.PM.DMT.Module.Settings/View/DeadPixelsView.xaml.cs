using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UnitySC.PM.DMT.Modules.Settings.ViewModel;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Modules.Settings.View
{
    /// <summary>
    /// Interaction logic for DeadPixelsView.xaml
    /// </summary>
    public partial class DeadPixelsView : UserControl
    {
        public DeadPixelsView()
        {
            InitializeComponent();



        }

        /// <summary>
        /// Text indiquant les infos selon la position de la souris
        /// </summary>
        public string StatusText
        {
            get { return (string)GetValue(StatusTextProperty); }
            set { SetValue(StatusTextProperty, value); }
        }
        public static readonly DependencyProperty StatusTextProperty =
            DependencyProperty.Register("StatusText", typeof(string), typeof(DeadPixelsView), new PropertyMetadata(null));


        private void theImage_MouseMove(object sender, MouseEventArgs e)
        {
            double grey = double.NaN;

            Point mousepos = e.GetPosition(theImage);

            var imageSource = (BitmapSource)theImage.Source;

            if (imageSource != null)
            {
                int x = (int)(mousepos.X * imageSource.Width / theImage.ActualWidth);
                int y = (int)(mousepos.Y * imageSource.Height / theImage.ActualHeight);

                if (0 <= x && x < imageSource.Width && 0 <= y && y < imageSource.Height)
                {
                    Color c = imageSource.GetPixelColor(x, y);
                    grey = c.B;
                }
            }

            if (double.IsNaN(grey))
                StatusText = "";
            else
                StatusText = String.Format("X: {0}    Y: {1}    Grey: {2}", (int)mousepos.X, (int)mousepos.Y, grey);
        }

        //private void theImage_SourceUpdated(object sender, DataTransferEventArgs e)
        //{
        //    DeadPixelsZoom.MinScale = 0;
        //    DeadPixelsZoom.FitToBounds();
        //    DeadPixelsZoom.MinScale = DeadPixelsZoom.Scale;
        //}

        private void theImage_MouseLeave(object sender, MouseEventArgs e)
        {
            StatusText = "";
        }

        //private void theImage_SourceUpdated_1(object sender, DataTransferEventArgs e)
        //{
        //    DeadPixelsZoom.MinScale = 0;
        //    DeadPixelsZoom.FitToBounds();
        //    DeadPixelsZoom.MinScale = DeadPixelsZoom.Scale;
        //}

        //private void Grid_SourceUpdated(object sender, DataTransferEventArgs e)
        //{

        //}

        //private void Grid_TargetUpdated(object sender, DataTransferEventArgs e)
        //{

        //}

        private void theImage_TargetUpdated(object sender, DataTransferEventArgs e)
        {
           
        }

        private void theImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                DeadPixelsZoom.MinScale = 0;
                DeadPixelsZoom.FitToBounds();
                DeadPixelsZoom.MinScale = DeadPixelsZoom.Scale;
            }), System.Windows.Threading.DispatcherPriority.ContextIdle);
        }

        private void deadPixelsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;
           
            DeadPixelsZoom.ZoomTo(new Point((((SelectableDeadPixel)((object[])e.AddedItems)[0]).AssociatedDeadPixel.X- DeadPixelsZoom.Viewport.Width/2) * DeadPixelsZoom.Scale, (((SelectableDeadPixel)((object[])e.AddedItems)[0]).AssociatedDeadPixel.Y-DeadPixelsZoom.Viewport.Height / 2) * DeadPixelsZoom.Scale));
        }
        
    }
}
