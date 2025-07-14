using System.Windows;
using System.Windows.Controls;

namespace ADC.View.Parameters
{
    /// <summary>
    /// Logique d'interaction pour ParametersExportDialog.xaml
    /// </summary>
    public partial class ParametersExportDialog : Window
    {
        public ParametersExportDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            listViewParams.SelectedItem = (sender as UserControl).DataContext;
        }
    }
}
