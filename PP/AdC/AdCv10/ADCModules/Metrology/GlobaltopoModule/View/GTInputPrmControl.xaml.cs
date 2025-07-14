using System.Windows;
using System.Windows.Controls;

namespace GlobaltopoModule.View
{
    /// <summary>
    /// Interaction logic for GTInputPrmControl.xaml
    /// </summary>
    public partial class GTInputPrmControl : UserControl
    {
        public GTInputPrmControl()
        {
            InitializeComponent();
        }

        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            areas.SelectedItem = (sender as Grid).DataContext;
        }
    }
}
