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

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures
{
    /// <summary>
    /// Interaction logic for MeasurePointsView.xaml
    /// </summary>
    public partial class MeasurePointsView : UserControl
    {
        public MeasurePointsView()
        {
            InitializeComponent();
        }

        private void ListBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MeasuresList.SelectedItem != null)
            {
                PopupMeasures.IsOpen = false;
            }
        }
      
        
    }
}
