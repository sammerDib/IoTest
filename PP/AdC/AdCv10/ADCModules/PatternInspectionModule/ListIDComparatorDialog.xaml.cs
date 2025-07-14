using System.Windows;

namespace PatternInspectionModule
{
    /// <summary>
    /// Logique d'interaction pour ListIDComparatorDialog.xaml
    /// </summary>
    public partial class ListIDComparatorDialog : Window
    {
        private ListIDComparatorViewModel ViewModel { get { return (ListIDComparatorViewModel)DataContext; } }

        public ListIDComparatorDialog()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Synchronize();
            DialogResult = true;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsNull = true;
            DialogResult = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
