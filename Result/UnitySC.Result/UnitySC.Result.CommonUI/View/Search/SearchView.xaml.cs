using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.Result.CommonUI.View.Search
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
            //Forcage du culture info en anglais pour avoir le calendrier en anglais.
            var culture = CultureInfo.InvariantCulture; //new CultureInfo("en-UK");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        private void startDate_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.Value = null;
        }

        private void endDate_Click(object sender, RoutedEventArgs e)
        {
            EndDatePicker.Value = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}