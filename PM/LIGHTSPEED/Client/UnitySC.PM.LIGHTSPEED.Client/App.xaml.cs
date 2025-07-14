using System.Windows;

namespace UnitySC.PM.LIGHTSPEED.Client
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Bootstrapper.Register();
        }
    }
}
