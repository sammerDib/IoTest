using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnitySC.Shared.Tools;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UnitySC.PM.DMT.Service.Implementation.Curvature;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Tools.TopoCalib;
using System.Globalization;

namespace UnitySC.PM.DMT.Tools.TopoCalibExlorer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = new ExplorerVM();
            InitializeComponent();
        }
    }
}
