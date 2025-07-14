using System.Diagnostics;
using System.Reflection;
using System.Windows;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.LIGHTSPEED.Client
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();
            _logger.Information("******************************************************************************************");
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _logger.Information("LIGHTSPEED CLIENT Version: " + version);
            _logger.Information("******************************************************************************************");

            Title = Title + " - " + version;

            DataContext = ClassLocator.Default.GetInstance<MainViewModel>();
        }
    }
}
