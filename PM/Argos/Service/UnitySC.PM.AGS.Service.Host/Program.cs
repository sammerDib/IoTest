using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;

using UnitySC.PM.AGS.Hardware.Manager;
using UnitySC.PM.AGS.Service.Implementation;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.LibMIL;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.AGS.Service.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var a = Init(args);
            Task.WaitAll(a); //Now Waiting
            Console.WriteLine("Exiting CommandLine");

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }

        private static async Task Init(string[] args)
        {
            Bootstrapper.Register(args);
            ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            _logger.Information("******************************************************************************************");
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _logger.Information("Argos SERVER Version: " + version);
            _logger.Information("******************************************************************************************");

            AllocateMilIfNeeded();

            if (Environment.UserInteractive)
            {
                try
                {
                    _logger.Information("Initializing hardware...");

                    _logger.Information("Argos console service starting...");
                    ArgosServer _hlsService = ClassLocator.Default.GetInstance<ArgosServer>();
                    _hlsService.Start();

                    _logger.Information("Waiting for client connection...");
                    var globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();

                    _logger.Information("Hardware Initialisation ...");

                    var hardwareManager = ClassLocator.Default.GetInstance<ArgosHardwareManager>();
                    hardwareManager.Init();

                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();
                    _hlsService.Stop();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Argos service error");
                }

                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new AgsWindowsService()
                };

                _logger.Information("Start Argos Windows service");
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
