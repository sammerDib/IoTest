using System.Windows;
using System.Windows.Controls;

namespace BasicModules.Grading.ClassGrading.View
{
    /// <summary>
    /// Interaction logic for ClassGradingControl.xaml
    /// </summary>
    public partial class ClassGradingControl : UserControl
    {
        public ClassGradingControl()
        {
            InitializeComponent();
        }

        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            rules.SelectedItem = (sender as Grid).DataContext;
        }
    }
}
