using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using UnitySC.PM.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var app = (App)Application.Current;

            Title = Title + (ClassLocator.Default.GetInstance<IClientConfigurationManager>().IsWaferLessMode ? " WAFER LESS" : string.Empty) + " - " + app.AppVersion;
            DataContext = ClassLocator.Default.GetInstance<MainViewModel>();
        }
    }
}
