using System.Windows;

using UnitySC.Result.CommonUI.ViewModel;
using UnitySC.Shared.Tools;

namespace UnitySC.Result.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClassLocator.Default.GetInstance<MainResultVM>().InitRessources();
        }
    }
}