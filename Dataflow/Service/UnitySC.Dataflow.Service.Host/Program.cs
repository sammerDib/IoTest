using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading.Tasks;

using UnitySC.dataflow.Service.Host;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.Dataflow.Service.Host
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;

            var exeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string trace = null;
            string CurDir = Directory.GetCurrentDirectory();
            try
            {
                string ExeDir = Path.GetDirectoryName(exeName);
                //Set the current directory.
                Directory.SetCurrentDirectory(ExeDir);
            }
            catch (Exception ex)
            {
                trace = ex.StackTrace;
            }
            finally
            {
                CurDir = Directory.GetCurrentDirectory();
            }

            try
            {

                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

                Bootstrapper.Register(args);
                ILogger logger = ClassLocator.Default.GetInstance<ILogger<object>>();

                logger.Information("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                logger.Information("  DataFlow SERVER Version : " + version);
                logger.Information("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                if (!string.IsNullOrEmpty(trace))
                {
                    logger.Error($"Current dir Modification has failed\n{trace}");
                    logger.Warning($"exe path = {exeName}");
                    logger.Warning($"working dir = {CurDir}");
                }

                var dfServer = ClassLocator.Default.GetInstance<DataflowServer>();
                dfServer.Start();
                ClassLocator.Default.GetInstance<ApplicationFDCs>().Register();

                // Used to create an intance of DataCollection Convert and trying to import the mef components before starting the dataflow
                var dcConvert = ClassLocator.Default.GetInstance<IDataCollectionConvert>();


                if ((!Environment.UserInteractive))
                {
                    Program.RunAsAService(args, logger);
                }
                else
                {
                    Program.RunAsAConsole(args, logger);
                }
                dfServer.Stop();
            }
            catch (Exception ex)
            {
                if (Environment.UserInteractive)
                {
                    var saveBack = Console.BackgroundColor;
                    var saveFront = Console.ForegroundColor;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("###");
                    Console.WriteLine("Initialisation error with args " + string.Join("", args.Skip(1).ToArray()));
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("###");
                    Console.BackgroundColor = saveBack;
                    Console.ForegroundColor = saveFront;

                    Console.WriteLine("Press enter to exit");
                    Console.ReadLine();
                }
                ClassLocator.Default.GetInstance<ILogger<object>>()?.Information("DATAFLOW server is terminated abnormally");
                ClassLocator.Default.GetInstance<ILogger<object>>()?.Error($"{ex.Message}\n### StackTrace :\n{ex.StackTrace}\n####");
                Environment.Exit(1); // exit abnormally     
            }

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            ClassLocator.Default.GetInstance<ILogger<object>>()?.Information("Dataflow server is terminated normally");
            Environment.Exit(0); //Exit normally           
        }

        private static void RunAsAService(string[] args, ILogger logger)
        {
            logger.Information("----------------------------------");
            logger.Information("Start DATAFLOW Windows service");

            logger.Verbose("      => Verbose logging activated");
            logger.Debug("      => Debug logging activated");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DataflowWindowsService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void RunAsAConsole(string[] args, ILogger logger)
        {
            Console.WriteLine(".");
            try
            {
                // Check if service not already running in a "hidden way"
                try
                {
                    ServiceController sc = new ServiceController(DataflowServiceInstaller.ServiceName);
                    if (sc.Status != ServiceControllerStatus.Stopped)
                    {
                        logger.Error(" DATAFLOW Service is currently RUNNING");
                        logger.Error($"** Please Stop <{DataflowServiceInstaller.ServiceName}> before launch Dataflow as console **\n");
                        throw new Exception("Dataflow service already running");
                    }
                }
                catch (ArgumentException) { }       // only in case of service not installed (According to doc not seen)
                catch (InvalidOperationException)   // only in case of service not installed
                {
                    // service name is invalid
                    // this service is not installed on this machine
                }

                logger.Information("----------------------------------");
                logger.Information("Start DATAFLOW Console server ");

                logger.Verbose("      => Verbose logging activated");
                logger.Debug("      => Debug logging activated");
                Console.WriteLine(".");

                Console.WriteLine("Press enter to stop DF Server");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Dataflow console error");
            }
            finally
            {
                Console.WriteLine("Press enter to quit");
               
                Console.ReadLine();
            }
        }
    }
}
