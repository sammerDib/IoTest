using System;
using System.Collections.Generic;
using System.Linq;
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

using Org.BouncyCastle.Asn1.Ocsp;

using UnitySC.Shared.Tools;

namespace TestRecipeRunLiveView
{
    /// <summary>
    /// Interaction logic for TestLiveMainWindow.xaml
    /// </summary>
    public partial class TestLiveMainWindow : Window
    {
        public TestLiveMainWindow()
        {
            InitializeComponent();
            DataContext = ClassLocator.Default.GetInstance<TestLiveMainViewModel>();
        }
    }
}
