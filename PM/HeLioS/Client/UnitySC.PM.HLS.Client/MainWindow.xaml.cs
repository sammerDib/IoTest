using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.HLS.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();
            _logger.Information("******************************************************************************************");
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _logger.Information("HeLioS client Version: " + version);
            _logger.Information("******************************************************************************************");

            Title = "HeLioS - " + version;
            DataContext = ClassLocator.Default.GetInstance<MainViewModel>();
        }
    }
}
