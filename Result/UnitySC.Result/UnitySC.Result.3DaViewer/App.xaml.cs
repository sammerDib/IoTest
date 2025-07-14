using System.Windows;

namespace UnitySC.Result._3DaViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Bootstrapper.Register();
        }
        #region Properties

        public static App Instance => Current as App;

        public MainWindowVM MainWindowViewModel { get; private set; }

        #endregion

        #region Overrides of Application

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindowViewModel = new MainWindowVM();
            MainWindow = new MainWindow { DataContext = MainWindowViewModel };
            MainWindow.Show();

            MainWindowViewModel.LoadFile(e.Args);
        }

        #endregion
    }
}
