using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Xml.Serialization;

using AppLauncher.ViewModel;

using UnitySC.Shared.Tools;

namespace AppLauncher.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public String WindowTitle { get; set; } = "UnitySC Launcher";

        public MainWindow()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;

            WindowTitle += $" - v {version}";

            InitializeComponent();

            DataContext = new MainWindowVM();

            Closing +=  (DataContext as MainWindowVM).OnWindowClosing;
        }

#if DEBUG
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var launcherConfig=new LauncherConfig();
            launcherConfig.DisplayStopAll = true;
            launcherConfig.Applications = new List<LauncherApplicationConfig>();
            launcherConfig.Services = new List<LauncherServiceConfig>();

            var launcherService1 = new LauncherServiceConfig();
            launcherService1.Name = "Service1";
            launcherService1.DisplayInLauncher = true;
            launcherService1.Description = "Description Service 1";
            launcherService1.Path = @"C\Temp\Service1.exe /I";
            launcherService1.ShowConsoleWindow = false;
            launcherService1.IsConsoleMode = true;
            launcherService1.DelayBeforeLaunchingNextService = 20;
            launcherConfig.Services.Add(launcherService1);

            var launcherService2 = new LauncherServiceConfig();
            launcherService2.Name = "Service2";
            launcherService2.DisplayInLauncher = true;
            launcherService2.Description = "Description Service 2";
            launcherService2.Path = @"C\Temp\Service2.exe /I";
            launcherService2.Arguments = "/I";
            launcherService2.ServiceName = "Service2";
            launcherService2.IsConsoleMode = false;
            launcherService2.ShowConsoleWindow = false;
            launcherConfig.Services.Add(launcherService2);

            var launcherApplication1 = new LauncherApplicationConfig();
            launcherApplication1.Name = "Application1";
            launcherApplication1.Description = "Description Application 1";
            launcherApplication1.Path = @"C:\Program Files\Notepad++\notepad++.exe";
            launcherApplication1.Arguments = @"/Help";

            launcherApplication1.ServiceDependencies = new List<string>
            {
                "Service1",
                "Service2"
            };
            launcherConfig.Applications.Add(launcherApplication1);
            string filePath = @"C:\Temp\LauncherConfig.xml";
            XML.Serialize(launcherConfig, filePath);
        }
#endif

    }
}
