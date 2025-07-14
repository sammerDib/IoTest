using System.Windows;

namespace UnitySC.Result.Client
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