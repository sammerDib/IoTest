using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UnitySC.Shared.UI.Controls.ZoomboxImage
{
    /// <summary>
    /// Interaction logic for ZoomboxImageWithButtons.xaml
    /// </summary>
    public partial class ZoomboxImageWithButtons : UserControl
    {
        public ZoomboxImageWithButtons()
        {
            InitializeComponent();
        }

        //=================================================================
        // Dependency properties
        //=================================================================
        public BitmapSource ImageSource
        {
            get { return (BitmapSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapSource), typeof(ZoomboxImageWithButtons), new PropertyMetadata(null));
    }
}