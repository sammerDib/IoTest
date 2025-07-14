using System.Windows;

namespace ResultsRegisterSimulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Bootstrapper.Register();
        }
    }
}
