using System.Windows;
using System.Windows.Controls;

namespace ADC.View.Parameters
{
    /// <summary>
    /// Logique d'interaction pour ParametersSimplifiedView.xaml
    /// </summary>
    public partial class ParametersSimplifiedView : UserControl
    {
        public ParametersSimplifiedView()
        {
            InitializeComponent();
        }


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ListBox lb = (ListBox)sender;
                lb.ScrollIntoView(e.AddedItems[0]);
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
