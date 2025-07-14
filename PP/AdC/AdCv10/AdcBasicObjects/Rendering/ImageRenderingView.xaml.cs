using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UnitySC.Shared.Tools;

namespace AdcBasicObjects.Rendering
{
    /// <summary>
    /// Interaction logic for ImageRendering.xaml
    /// </summary>
    public partial class ImageRenderingView : UserControl
    {
        private static ImageRenderingView _defaultInstance;
        public static ImageRenderingView DefaultInstance
        {
            get
            {
                if (_defaultInstance == null)
                    _defaultInstance = new ImageRenderingView();
                return _defaultInstance;
            }
        }

        public ImageRenderingView()
        {
            InitializeComponent();
        }

        private void FitContent(object sender, RoutedEventArgs e)
        {
            ZoomBox.FitToBounds();
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            ZoomBox.Scale += 0.01;
        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            ZoomBox.Scale -= 0.01;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (double.IsNaN(ZoomBox.Width))
                ZoomBox.Width = ZoomBox.ActualWidth;// TODO a virer quand on intègrera la version de XCeed Toolkit qui corrige le plantage quand ZoomBox.Width et ZoomBox.Height sont nuls
            if (double.IsNaN(ZoomBox.Height))
                ZoomBox.Height = ZoomBox.ActualHeight; // Cf https://xceed.com/release-notes/
            base.OnRender(drawingContext);
        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
                overflowGrid.Visibility = Visibility.Collapsed;

            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
                mainPanelBorder.Margin = new Thickness();
        }

        //=================================================================
        // Status X/Y/Grey
        //=================================================================
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(Image);

            // Get Grey level
            //...............
            Point pti = e.GetPosition(Image);

            double grey;
            try
            {
                Color c = ((BitmapSource)Image.Source).GetPixelColor((int)pti.X, (int)pti.Y);
                grey = c.B;
            }
            catch (Exception)
            {
                // hors de l'image
                grey = double.NaN;
            }

            // Display Grey Level
            //...................
            StatusTextBox.Text = String.Format("X: {0}    Y: {1}    Grey: {2}", (int)pti.X, (int)pti.Y, grey);
        }

        //=================================================================
        // Change Zoombox default ViewFinder icon
        //=================================================================
        private bool _zoomBoxLoaded;
        private void ZoomBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (_zoomBoxLoaded)
                return;
            _zoomBoxLoaded = true;

            Image showViewFinderGlyphImage = (Image)ZoomBox.FindVisualChildByName("ShowViewFinderGlyphImage");
            if (showViewFinderGlyphImage != null)
            {
                System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("AdcBasicObjects.Rendering.ViewFinder.png");
                BitmapSource bmp = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                showViewFinderGlyphImage.Source = bmp;
            }

            ZoomBox.FitToBounds();
        }


    }
}
