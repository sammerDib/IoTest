using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.HLS.Hardware.Manager;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Data.Configuration;

namespace UnitySC.PM.HLS.Service.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            var a = Init(args);
            Task.WaitAll(a); //Now Waiting
            Console.WriteLine("Exiting CommandLine");

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }

        private static async Task Init(string[] args)
        {
            Bootstrapper.Register(args);

            var configuration = ClassLocator.Default.GetInstance<IServiceConfigurationManager>();
            ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            _logger.Information("******************************************************************************************");
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _logger.Information("HeLioS SERVER Version: " + version);
            _logger.Information("******************************************************************************************");

            _logger.Information($"Configuration manager status: {configuration.GetStatus()}");

            AllocateMilIfNeeded();

            await Task.Delay(250);
            if (Environment.UserInteractive)
            {
                try
                {
       
                    _logger.Information("HeLioS console service starting...");
                    HlsServer _hlsService = ClassLocator.Default.GetInstance<HlsServer>();
                    _hlsService.Start();

                    var globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();

                    _logger.Information("Waiting for client connection...");


                    _logger.Information("Hardware Initialisation ...");
                    var hardwareManager = ClassLocator.Default.GetInstance<HlsHardwareManager>();
                    hardwareManager.Init();
                   
                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                    _hlsService.Stop();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "HeLioS service error");
                }

                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new HlsWindowsService()
                };

                _logger.Information("Start HeLioS Windows service");
                ServiceBase.Run(ServicesToRun);
            }
        }

        private static void AllocateMilIfNeeded()
        {
            ILogger logger = ClassLocator.Default.GetInstance<ILogger<object>>();

            var pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            if (pmConfiguration.UseMatroxImagingLibrary)
            {
                logger.Information("Matrox Imaging Library activated in PM configuration");
                Mil.Instance.Allocate();
                return;
            }
            logger.Information("Matrox Imaging Library is deactivated in PM configuration");
        }
    }
}
