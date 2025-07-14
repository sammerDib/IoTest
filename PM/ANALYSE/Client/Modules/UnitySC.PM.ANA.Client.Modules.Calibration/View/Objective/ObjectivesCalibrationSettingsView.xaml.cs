using System.Windows;
using System.Windows.Controls;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.View
{
    /// <summary>
    /// Interaction logic for ObjectivesCalibrationSettingsView.xaml
    /// </summary>
    public partial class ObjectivesCalibrationSettingsView : UserControl
    {
        public ObjectivesCalibrationSettingsView()
        {
            InitializeComponent();
        }

        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            objectivesList.SelectedItem = (sender as Grid).DataContext;
        }
    }
}
