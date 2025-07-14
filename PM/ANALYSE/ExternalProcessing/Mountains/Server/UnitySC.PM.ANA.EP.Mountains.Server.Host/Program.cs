using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Server.Implementation;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.EP.Mountains.Server.Host
{
    internal class Program
    {
        private static void Main(string[] args)
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

                logger.Information("------------------------------------------------");
                logger.Information("  EP Mountains SERVER Version: : " + version);
                logger.Information("------------------------------------------------");

                if (!string.IsNullOrEmpty(trace))
                {
                    logger.Error($"Current dir Modification has failed\n{trace}");
                    logger.Warning($"exe path = {exeName}");
                    logger.Warning($"working dir = {CurDir}");
                }

                CheckConfig();

                if ((!Environment.UserInteractive))
                {
                    Program.RunAsAService(args, logger);
                }
                else
                {
                    Program.RunAsAConsole(args, logger);
                }
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
                ClassLocator.Default.GetInstance<ILogger<object>>()?.Information(" EP Mountains SERVER is terminated abnormally");
                ClassLocator.Default.GetInstance<ILogger<object>>()?.Error($"{ex.Message}\n### StackTrace :\n{ex.StackTrace}\n####");
                Environment.Exit(1); // exit abnormally     
            }

            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

           /* CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            //// wait to test on tool - delete the following code when validate on tool with Moutains  
            //var a = Init(args);
            //Task.WaitAll(a); //Now Waiting
            try
            {
                // new code to be validated on tool 
                Task.Run(async () =>
                {
                    await Init(args); //Now Waiting
                }).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Caught Exception: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Exiting CommandLine");
            }

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;*/
        }


        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            ClassLocator.Default.GetInstance<ILogger<object>>()?.Information("EP Mountains SERVER is terminated normally");
            Environment.Exit(0); //Exit normally           
        }

        private static void RunAsAService(string[] args, ILogger logger)
        {
            logger.Information("----------------------------------");
            logger.Information("Start EP MOUNTAINS Windows service");

            logger.Verbose("      => Verbose logging activated");
            logger.Debug("      => Debug logging activated");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MountainsWindowsService()
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
                    ServiceController sc = new ServiceController(MountainsServiceInstaller.ServiceName);
                    if (sc.Status != ServiceControllerStatus.Stopped)
                    {
                        logger.Error(" EP Mountains Service is currently RUNNING");
                        logger.Error($"** Please Stop <{MountainsServiceInstaller.ServiceName}> before launch EP Mountains as console **\n");
                        throw new Exception("EP Mountains service already running");
                    }
                }
                catch (ArgumentException) { }       // only in case of service not installed (According to doc not seen)
                catch (InvalidOperationException)   // only in case of service not installed
                {
                    // service name is invalid
                    // this service is not installed on this machine
                }

                logger.Information("----------------------------------");
                logger.Information("Start  EP MOUNTAINS Console server ");

                logger.Verbose("      => Verbose logging activated");
                logger.Debug("      => Debug logging activated");
                Console.WriteLine(".");

                var mountainsServer = ClassLocator.Default.GetInstance<MountainsServer>();
                mountainsServer.Start();
                logger.Information("ActiveX host start ...");

                Console.WriteLine("Press enter to stop Mountains Server");
                Console.ReadLine();
                mountainsServer.Stop();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "EP MOUNTAINS console error");
            }
            finally
            {
                Console.WriteLine("Press enter to quit");

                Console.ReadLine();
            }
        }


        private static async Task Init(string[] args)
        {
            try
            {
                Bootstrapper.Register(args);
                ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();
                CheckConfig();
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                _logger.Information("******************************************************************************************");
                Assembly assembly = Assembly.GetExecutingAssembly();
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fileVersionInfo.ProductVersion;
                _logger.Information("Mountains SERVER Version: " + version);
                _logger.Information("******************************************************************************************");
                       

                if (Environment.UserInteractive)
                {
                    try
                    {
                        MountainsServer _mountainsServer = ClassLocator.Default.GetInstance<MountainsServer>();
                        _mountainsServer.Start();
                        _logger.Information("ActiveX host start ...");                        
                        await Task.Delay(100);
                        Console.WriteLine("Press enter to stop");
                        Console.ReadLine();
                        _mountainsServer.Stop();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Mountains service error");
                    }
                }
                else
                {
                    /*
                     * 
                     Todo (note de rti à FDJ : todo ok  but todo what ?

                    */
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Initialisation error with args " + string.Join("", args.Skip(1).ToArray()));
                Console.WriteLine(ex.ToString());

            }
            finally
            {
                Console.WriteLine("--------------------");
                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
        }

        private static void CheckConfig()
        {
            try
            {
                var configuration = ClassLocator.Default.GetInstance<MountainsConfiguration>();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error in MountainsServiceConfiguration {ex.Message}");
                throw new Exception($"Error in MountainsServiceConfiguration {ex.Message}", ex);
            }
        }
    }
}
