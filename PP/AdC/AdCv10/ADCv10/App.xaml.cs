using System;
using System.Configuration;
using System.Linq;
using System.Windows;

using AdcTools;
using AdcTools.Serilog;

using Serilog;

using UnitySC.PP.Shared.Configuration;
using UnitySC.Shared.Tools;

namespace ADC
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AppArguments CommandLineArgs { get; private set; }

        private static string _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string Version { get { return _version; } }
        public static bool StartupAsOperator = false;

        public App()
        {
            PathString logfolder = ConfigurationManager.AppSettings["LogFolder"];
            PathString logfile = PathString.GetExeFullPath().ChangeExtension(".log").Filename;
            Log.Logger = new LoggerConfiguration()
                               .ReadFrom.AppSettings()
                               .WriteTo.File(path: logfolder / logfile,
                                            rollOnFileSizeLimit:true,
                                            fileSizeLimitBytes: 20971520,
                                            retainedFileCountLimit: 100)
                               .WriteTo.Console()
                               .WriteTo.StringSink()
                               .CreateLogger();
            Log.Information("\n\nADCv" + Version + " starting\n\n");

            // NOTE de RTI TODOD : comment chopper les arguiment d'appel ici
            //  var currentConfiguration = new PPServiceConfigurationManager(args);
            //  var currentConfiguration = new PPServiceConfigurationManager("-c 4MET2223 -sa".Split(' '));

            string[] args = null;
            args = Environment.GetCommandLineArgs();
            if (args != null && args.Length > 1)
                args = args.Skip(1).ToArray();
            
            Bootstrapper.Register(args);
            Log.Information("\n\nADC Bootstrapper Register ok\n\n");

            // // Call DispatcherHelper.Initialize() 
            // Log.Information("\n\nCall DispatcherHelper.Initialize()\n\n");
            // GalaSoft.MvvmLight.Threading.DispatcherHelper.Initialize();

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

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture; // new System.Globalization.CultureInfo("en-US"); 
        }

        public void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            CommandLineArgs = new AdcTools.AppArguments(e.Args);

            MergeContext.Context.Initializer.Init();
            ADCEngine.ADC.Instance.Init();

        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;

            ExceptionMessageBox.Show("Fatal error", ex, isFatal: true);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        public void Application_Exit(object sender, ExitEventArgs e)
        {
            ADCEngine.ADC.Instance.Shutdown();
        }
    }
}
