using System.Windows;

using ConfigurationManager.ViewModel;

namespace ConfigurationManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel mainViewModel = new MainViewModel();
            mainViewModel.Init();
            DataContext = mainViewModel;
        }
    }
}
