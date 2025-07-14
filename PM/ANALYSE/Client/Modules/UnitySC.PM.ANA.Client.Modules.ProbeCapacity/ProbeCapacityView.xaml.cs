using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UnitySC.PM.ANA.Client.Modules.ProbeCapacity
{
    /// <summary>
    /// Interaction logic for ProbeCapacityView.xaml
    /// </summary>
    public partial class ProbeCapacityView : UserControl
    {
        public ProbeCapacityView()
        {
            InitializeComponent();
        }

        private void Grid_GotFocus(object sender, RoutedEventArgs e)
        {
            probeList.SelectedItem = (sender as Grid).DataContext;
        }
    }
}
