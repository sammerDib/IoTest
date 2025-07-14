using System.Diagnostics;
using System.Reflection;
using System.Windows;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();
            _logger.Information("******************************************************************************************");
            var assembly = Assembly.GetExecutingAssembly();
            string version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            _logger.Information($"{Title} CLIENT Version: {version}");
            _logger.Information("******************************************************************************************");

            Title = Title + " - " + version;
            DataContext = ClassLocator.Default.GetInstance<MainViewModel>();
        }
    }
}
