using System.Windows;

using UnitySC.Shared.Tools;

namespace ResultsRegisterSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClassLocator.Default.GetInstance<MainRegisterVM>().InitRessources();
        }
    }
}
