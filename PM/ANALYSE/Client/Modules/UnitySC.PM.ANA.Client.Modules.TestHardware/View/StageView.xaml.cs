using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.View
{
    /// <summary>
    /// Interaction logic for StageView.xaml
    /// </summary>
    public partial class StageView : UserControl
    {
        public StageView()
        {
            InitializeComponent();
        }

        private void DoubleValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void IntegerValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9.-]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ListBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (RefsList.SelectedItem!=null)
            {
                PopupReferences.IsOpen = false;
            }
        }

   
    }
}
