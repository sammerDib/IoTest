using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.ServiceProcess;
using UnitySC.PM.LIGHTSPEED.Service.Implementation;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.LIGHTSPEED.Service.Host
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            Bootstrapper.Register();
            SerilogInit.InitWithCurrentAppConfig();
            ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            _logger.Information("******************************************************************************************");
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _logger.Information("LIGHTSPEED SERVER Version: " + version);
            _logger.Information("******************************************************************************************");

            if (Environment.UserInteractive)
            {
                try
                {
                    _logger.Information("LIGHTSPEED console service starting...");

                    LSServer _lsServer = ClassLocator.Default.GetInstance<LSServer>();
                    _lsServer.Start();

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                    _lsServer.Stop();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "LS service error");
                }

                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new LightspeedWindowsService()
                };

                _logger.Information("Start LIGHTSPEED Windows service");
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
