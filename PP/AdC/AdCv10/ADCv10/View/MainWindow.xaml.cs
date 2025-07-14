using System.Windows;

using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace ADC.View
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var result = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<UnitySC.PM.Shared.UI.Connection.ConnectionWindow>(
new UnitySC.PM.Shared.UI.Connection.ConnectionViewModel(ClassLocator.Default.GetInstance<IUserSupervisor>(), ClassLocator.Default.GetInstance<ILogger>()));
            if (!result.HasValue || !result.Value)
                Application.Current.Shutdown();
        }
    }
}
