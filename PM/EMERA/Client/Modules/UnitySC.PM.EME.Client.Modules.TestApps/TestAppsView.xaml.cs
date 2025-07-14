using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace UnitySC.PM.EME.Client.Modules.TestApps
{
    /// <summary>
    /// Interaction logic for TestAppsView.xaml
    /// </summary>
    public partial class TestAppsView : UserControl
    {
        public TestAppsView()
        {
            InitializeComponent();
        }

        private void OpenCalibrationFile(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
