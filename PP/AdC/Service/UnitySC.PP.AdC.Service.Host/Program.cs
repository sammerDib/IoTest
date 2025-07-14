using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.ServiceProcess;
using System.Threading.Tasks;

using UnitySC.PP.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
namespace UnitySC.PP.ADC.Service.Host
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;



            var a = Init();
            Task.WaitAll(a); //Now Waiting
            Console.WriteLine("Exiting CommandLine");

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.WriteLine("AssemblyResolve: " + args.Name);

            return null;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            // Console.WriteLine("AssemblyLoad:" + args.LoadedAssembly.FullName+" - "+ args.LoadedAssembly.Location);

        }

        private static async Task Init()
        {
            Bootstrapper.Register();
            ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            _logger.Information("******************************************************************************************");
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _logger.Information("ADC SERVER Version: " + version);
            _logger.Information("******************************************************************************************");
 
            AppParameter.Instance.Init( (p) =>
            {
                  switch (p)
                  {

                      case "PPHostIAdcExecutor":
                          return ConfigurationManager.AppSettings["PPHostIAdcExecutor"];
                      //return "net.tcp://localhost:2250/AdcExecutor"; //[TODO] mettre en parametre

                      case "PathUIResourcesXml":
                          return PathString.GetExecutingAssemblyPath().Directory;
                      case "PathModuleDll":
                          return ConfigurationManager.AppSettings["PathModuleDll"];

                          //return @"C:\Users\n.chaux\source\repos\UnityControl\PP\ADC\Output\Debug";

                          // return @"C:\Users\n.chaux\source\ADC\ADC\Output\Debug";
                          // return PathString.GetExecutingAssemblyPath().Directory;

                  }

                  return null;
            });

            await Task.Delay(10);

            AllocateMilIfNeeded();

            if (Environment.UserInteractive)
            {
                try
                {
                    _logger.Information("Initializing hardware...");

                    _logger.Information("ADC console service starting...");
                    ADCServer _hlsService = ClassLocator.Default.GetInstance<ADCServer>();
                    _hlsService.Start();

                    _logger.Information("Waiting for client connection...");
                    // var globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();
                    // _logger.Information("Hardware Initialisation ...");

                    /* var hardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();
                    hardwareManager.Init();
                    todo create hardware configuration

                     */


                    string ln = "";

                    Console.WriteLine("Type 'test' or Press enter to stop");


                    while ((ln = Console.ReadLine()) != "")
                    {
                        if (ln == "test")
                        {

                            ADCEngine.IAdcExecutor ADCE = ClassLocator.Default.GetInstance<ADCEngine.IAdcExecutor>();

                            Interface.IADCService ADCS = ClassLocator.Default.GetInstance<Interface.IADCService>();

                            ADCS.ExecuteRecipe(@"..\data\AddReportRecipeb.adcrcp", @"..\data\Additionnalreport.ada");

                        }
                        await Task.Delay(250);
                    }

                    //Console.WriteLine("Press enter to stop");
                    //Console.ReadLine();
                    _hlsService.Stop();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "ADC service error");
                }

                Console.WriteLine("Press enter to quit");




            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new ADCWindowsService()
                };

                _logger.Information("Start ADC Windows service");
                ServiceBase.Run(ServicesToRun);
            }
        }

        private static void AllocateMilIfNeeded()
        {
            ILogger logger = ClassLocator.Default.GetInstance<ILogger<object>>();

            /*
            var pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            if (pmConfiguration.UseMatroxImagingLibrary)
            {
                logger.Information("Matrox Imaging Library activated in PM configuration");
                Mil.Instance.Allocate();
                return;
            }
            */
            logger.Information("Matrox Imaging Library is deactivated in PP configuration");

        }
    }
}
