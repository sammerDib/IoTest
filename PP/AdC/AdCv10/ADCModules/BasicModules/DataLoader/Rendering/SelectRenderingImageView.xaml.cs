using System.Windows;
using System.Windows.Controls;

namespace BasicModules.DataLoader
{
    /// <summary>
    /// Interaction logic for SelectRenderingImageView.xaml
    /// </summary>
    public partial class SelectRenderingImageView : Window
    {
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

        public SelectRenderingImageView()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListView lb = (ListView)sender;
                lb.Items.MoveCurrentTo(e.AddedItems[0]);
                lb.ScrollIntoView(e.AddedItems[0]);
            }
        }

    }
}
