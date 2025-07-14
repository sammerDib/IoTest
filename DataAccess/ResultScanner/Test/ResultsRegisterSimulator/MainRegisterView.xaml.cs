using System.Windows;
using System.Windows.Controls;

namespace ResultsRegisterSimulator
{
    /// <summary>
    /// Interaction logic for MainRegisterView.xaml
    /// </summary>
    public partial class MainRegisterView : UserControl
    {
        public MainRegisterView()
        {
            InitializeComponent();
        }

        //Evenement declenché par la selection d'un process module.
        private void RadioButton_GotFocus(object sender, RoutedEventArgs e)
        {
            listTools.SelectedItem = (sender as RadioButton).DataContext;
        }
    }
}
