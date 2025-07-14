using System.Windows;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.Views.Splashscreen
{
    /// <summary>
    /// Interaction logic for SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SplashScreenWindow"/> class.
        /// </summary>
        public SplashScreenWindow()
        {
            InitializeComponent();
        }

        private void SplashScreenWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
