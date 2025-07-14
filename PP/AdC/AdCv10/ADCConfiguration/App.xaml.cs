using System;
using System.Windows;

using UnitySC.PP.Shared.Configuration;
using UnitySC.Shared.Tools;

namespace ADCConfiguration
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string Version { get { return _version; } }

        public static Tools.AppArguments CommandLineArgs { get; private set; }

        public App()
        {

            Bootstrapper.Register();
            Services.Services.Instance.LogService.LogInfo("\n\nADCConfiguration version " + Version + " starting...\n\n", false);

            AppParameter.Instance.Init(
               (p) =>
               {

                   switch (p)
                   {
                       case "PPHostIAdcExecutor":
                           return "net.tcp://localhost:2250/AdcExecutor"; //[TODO] : en parametre
                       case "PathUIResourcesXml":
                           return PathString.GetExecutingAssemblyPath().Directory;
                       case "PathModuleDll":
                           return PathString.GetExecutingAssemblyPath().Directory;
                   }

                   return null;


               }
               );


        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CommandLineArgs = new Tools.AppArguments(e.Args);

            ADCEngine.ADC.Instance.LoadModules();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            Services.Services.Instance.LogService.LogError("UnhandledException: " + ex.ToString());
            Current.Dispatcher.Invoke((() => { AdcTools.ExceptionMessageBox.Show("UnhandledException: ", ex); }));
        }
    }
}
