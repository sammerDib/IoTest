using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace BasicModules.Edition.Rendering
{
    /// <summary>
    /// Interaction logic for WaferView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class WaferView : UserControl
    {
        public WaferView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ZoomBox.FitToBounds();
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            ZoomBox.GoHome();
        }

        private void FitContent(object sender, RoutedEventArgs e)
        {
            ZoomBox.FitToBounds();
        }

        private void Fill(object sender, RoutedEventArgs e)
        {
            ZoomBox.FillToBounds();
        }

        private void ZoomIn(object sender, RoutedEventArgs e)
        {
            ZoomBox.Scale += 0.01;

        }

        private void ZoomOut(object sender, RoutedEventArgs e)
        {
            ZoomBox.Scale -= 0.01;
        }

        private void myCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ZoomBox.FitToBounds();
        }

        private void CreateImage()
        {
            Image myImage = new Image();
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap(180, 180, 120, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            myImage.Source = bmp;

            // Add Image to the UI
            StackPanel myStackPanel = new StackPanel();
            myStackPanel.Children.Add(myImage);
            Content = myStackPanel;
        }


    }
}
