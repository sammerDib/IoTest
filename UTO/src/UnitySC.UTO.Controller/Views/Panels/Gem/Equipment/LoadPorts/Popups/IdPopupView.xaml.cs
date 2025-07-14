namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts.Popups
{
    /// <summary>
    /// Logique d'interaction pour IdPopupView.xaml
    /// </summary>
    public partial class IdPopupView
    {
        public IdPopupView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            IdTextBox.Focus();
        }
    }
}
