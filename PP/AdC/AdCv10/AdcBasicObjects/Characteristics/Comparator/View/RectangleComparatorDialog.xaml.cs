using System.Windows;

namespace AdcBasicObjects
{
    /// <summary>
    /// Interaction logic for RectangleComparatorDialog.xaml
    /// </summary>
    public partial class RectangleComparatorDialog : Window
    {
        private RectangleComparatorViewModel ViewModel { get { return (RectangleComparatorViewModel)DataContext; } }

        public RectangleComparatorDialog()
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
