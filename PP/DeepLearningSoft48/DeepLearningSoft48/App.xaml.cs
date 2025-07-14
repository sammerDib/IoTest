using System.Windows;

using DeepLearningSoft48.Services;

namespace DeepLearningSoft48
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ViewModelLocator ViewModelLocator = new ViewModelLocator();

            MainWindow = new MainWindow()
            {
                DataContext = ViewModelLocator.MainWindow

            };
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
