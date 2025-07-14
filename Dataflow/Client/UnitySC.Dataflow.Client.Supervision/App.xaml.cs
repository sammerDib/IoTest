using System.Windows;

using UnitySC.Shared.Tools;

namespace UnitySC.Dataflow.Client.Supervision
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //     StartupUri="MainWindow.xaml"  

        public App()
        {
            Bootstrapper.Register();

            var logger = ClassLocator.Default.GetInstance<UnitySC.Shared.Logger.ILogger>();
            logger.Information($"Démarrage Application");


            //App.Current.MainWindow = new MainWindow();


        }

    }
}
