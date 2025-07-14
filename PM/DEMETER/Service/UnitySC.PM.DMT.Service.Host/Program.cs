using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Host
{
    public static class Program
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
                ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();

                _logger.Information("******************************************************************************************");
                _logger.Information("DEMETER SERVER Version: " + version);
                _logger.Information("******************************************************************************************");

                if (!string.IsNullOrEmpty(trace))
                {
                    _logger.Error($"Current dir Modification has failed\n{trace}");
                    _logger.Warning($"exe path = {exeName}");
                    _logger.Warning($"working dir = {CurDir}");
                }

                if ((!Environment.UserInteractive))
                {
                    Program.RunAsAService(args, _logger);
                }
                else
                {
                    Program.RunAsAConsole(args, _logger);
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
                ClassLocator.Default.GetInstance<ILogger<object>>()?.Information("DEMETER server terminated abnormally");
                ClassLocator.Default.GetInstance<ILogger<object>>()?.Error($"{ex.Message}\n### StackTrace :\n{ex.StackTrace}\n####");
                Environment.Exit(1); // exit abnormally
            }
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            ClassLocator.Default.GetInstance<ILogger<object>>()?.Information("DEMETER server is terminated normally");
            Environment.Exit(0); //Exit normally
        }

        private static void RunAsAConsole(string[] args, ILogger logger)
        {
            Console.WriteLine(".");
            try
            {
                logger.Information("----------------------------------");
                logger.Information("Start DEMETER Console server ");

                logger.Verbose("      => Verbose logging activated");
                logger.Debug("      => Debug logging activated");
                Console.WriteLine(".");

                var dmtServer = ClassLocator.Default.GetInstance<DMTServer>();
                dmtServer.Start();

                Console.WriteLine("Press enter to stop Demeter Server");
                Console.ReadLine();
                dmtServer.Stop();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Demeter console error");
            }
            finally
            {
                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
        }

        private static void RunAsAService(string[] args, ILogger logger)
        {
            logger.Information("----------------------------------");
            logger.Information("Start DEMETER Windows service");

            logger.Verbose("      => Verbose logging activated");
            logger.Debug("      => Debug logging activated");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new DMTWindowsService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
