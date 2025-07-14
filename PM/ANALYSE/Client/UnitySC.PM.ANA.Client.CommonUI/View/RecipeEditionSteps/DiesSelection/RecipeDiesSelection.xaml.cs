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

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps;

namespace UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps
{
    /// <summary>
    /// Interaction logic for RecipeDiesSelectionView.xaml
    /// </summary>
    public partial class RecipeDiesSelectionView : UserControl
    {
        public RecipeDiesSelectionView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                WaferMapControl.Visibility = Visibility.Visible;
            }));
        }
    }
}
