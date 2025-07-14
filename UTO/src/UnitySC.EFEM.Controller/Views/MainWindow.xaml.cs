using System.Windows;
using System.Windows.Controls;

namespace UnitySC.EFEM.Controller.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Disable the default error template for all controls.
            // Validation templates are explicitly applied to some controls in styles.
            Validation.ErrorTemplateProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable));

            InitializeComponent();
        }
    }
}
