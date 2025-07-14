using System.Windows;

namespace AdcBasicObjects
{
    /// <summary>
    /// Interaction logic for BooleanComparatorDialog.xaml
    /// </summary>
    public partial class BooleanComparatorDialog : Window
    {
        private BooleanComparatorViewModel ViewModel { get { return (BooleanComparatorViewModel)DataContext; } }

        public BooleanComparatorDialog()
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
