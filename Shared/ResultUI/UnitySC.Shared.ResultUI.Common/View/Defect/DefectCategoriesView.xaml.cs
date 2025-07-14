using System.Windows;

namespace UnitySC.Shared.ResultUI.Common.View.Defect
{
    /// <summary>
    /// Interaction logic for DefectCategoryView.xaml
    /// </summary>
    public partial class DefectCategoriesView
    {
        public DefectCategoriesView()
        {
            InitializeComponent();
        }

        private void ClassGotFocus(object sender, RoutedEventArgs e)
        {
            //listViewClass.SelectedItem = (sender as Grid).DataContext;
        }
    }
}
