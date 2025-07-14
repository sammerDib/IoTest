using System.Windows;

namespace AdcBasicObjects
{
    /// <summary>
    /// Interaction logic for StringComparatorDialog.xaml
    /// </summary>
    public partial class StringComparatorDialog : Window
    {
        private StringComparatorViewModel ViewModel { get { return (StringComparatorViewModel)DataContext; } }

        public StringComparatorDialog()
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
