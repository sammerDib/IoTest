using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace WpfAppTestFlowManager
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

            var logger = ClassLocator.Default.GetInstance<ILogger>();
            logger.Information($"Démarrage Application");


            //App.Current.MainWindow = new MainWindow();


        }

    }
}
