using System.Windows;
using System.Windows.Controls;

using UnitySC.Shared.Tools;

namespace ADC.View.Parameters
{
    /// <summary>
    /// Logique d'interaction pour ParametersExpertView.xaml
    /// </summary>
    public partial class ParametersExpertView : UserControl
    {
        public ParametersExpertView()
        {
            InitializeComponent();
        }

        //=================================================================
        // Création des lignes de la grille
        //=================================================================
        private void listViewGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = (Grid)sender;

            grid.RowDefinitions.Clear();
            for (int i = 0; i < grid.Children.Count; i++)
            {
                var item = grid.Children[i] as FrameworkElement;
                System.Windows.Controls.Grid.SetRow(item, i);

                Control control = (Control)item.FindVisualChild(dp => dp is UserControl);
                var rowdef = new System.Windows.Controls.RowDefinition();
                if (control != null && control.VerticalAlignment == VerticalAlignment.Stretch)
                    rowdef.Height = new GridLength(100, GridUnitType.Star);
                else
                    rowdef.Height = new GridLength(0, GridUnitType.Auto);
                grid.RowDefinitions.Add(rowdef);
            }
        }


        /// <summary>
        ///¨Permet de séléctionner l'item de la liste lors du focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentPresenter_GotFocus(object sender, RoutedEventArgs e)
        {
            listViewParams.SelectedItem = (sender as ContentPresenter).DataContext;
        }
    }
}
