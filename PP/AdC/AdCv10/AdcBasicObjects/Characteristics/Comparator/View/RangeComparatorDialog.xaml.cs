using System.Windows;

namespace AdcBasicObjects
{
    /// <summary>
    /// Interaction logic for RangeComparatorDialog.xaml
    /// </summary>
    public partial class RangeComparatorDialog : Window
    {
        private RangeComparatorViewModel ViewModel { get { return (RangeComparatorViewModel)DataContext; } }

        public RangeComparatorDialog()
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
