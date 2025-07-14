using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;

using UnitySC.PM.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Host
{
    internal static class Program
    {
        // See https://learn.microsoft.com/en-us/windows/console/handlerroutine
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);
        private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);
        private static readonly SetConsoleCtrlEventHandler s_handler = Handler; // Keep a reference on delegate to avoid garbage collection

        const string ErrorFilePath = "errorAppCrash.txt";

        const int STD_INPUT_HANDLE = -10;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);

        private static void InterruptReadLine()
        {
            var handle = GetStdHandle(STD_INPUT_HANDLE);
            bool success = CancelIoEx(handle, IntPtr.Zero);
        }

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,       // No timeout
            CTRL_BREAK_EVENT = 1,   // No timeout  
            CTRL_CLOSE_EVENT = 2,   // system parameter SPI_GETHUNGAPPTIMEOUT, 5000ms
            CTRL_LOGOFF_EVENT = 5,  // system parameter SPI_GETWAITTOKILLTIMEOUT, 5000ms ou Service process system parameter SPI_GETWAITTOKILLSERVICETIMEOUT, 20000ms
            CTRL_SHUTDOWN_EVENT = 6,// system parameter SPI_GETWAITTOKILLTIMEOUT, 5000ms
        }

        private static void TerminateServer(AnaServer anaServer, CtrlType signal)
        {
            InterruptReadLine();

            var logger = anaServer?.Logger ?? ClassLocator.Default.GetInstance<ILogger<object>>();
            logger?.Information(signal.ToString() + " signal received. Closing Analyse server.");
            anaServer?.Stop();
            logger?.Information(signal.ToString() + " Analyse server successfully stopped.");

            Environment.Exit(0); // exit normally

        }

        private static bool Handler(CtrlType signal)
        {
            var anaServer = ClassLocator.Default.GetInstance<AnaServer>();
            var logger = anaServer?.Logger ?? ClassLocator.Default.GetInstance<ILogger<object>>();
            switch (signal)
            {

                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                    TerminateServer(anaServer, signal);
                    return false;

                default:
                    logger?.Warning("Signal not handle");
                    break;
            }
            return false;
        }

        static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            WriteErrorFile();
        }

        static void WriteErrorFile()
        {
            File.AppendAllLines(ErrorFilePath, new List<string>() { DateTime.UtcNow + "Analyse server fatal error." });
        }

        private static void Main(string[] args)
        {
            // Catch all unhandled exceptions, except StackOverFlow not thrown by user code
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;

            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            string trace = null;
            string CurDir = null;

            try
            {
                string ExeDir = Path.GetDirectoryName(exeName);
                Directory.SetCurrentDirectory(ExeDir);
            }
            catch (Exception ex)
            {
                trace = ex.StackTrace;
                CurDir = Directory.GetCurrentDirectory();
            }

            try
            {
                Bootstrapper.Register(args);
                ILogger logger = ClassLocator.Default.GetInstance<ILogger<object>>();

                logger.Information("**********************************************************************************");
                logger.Information("  ANALYSE SERVER Version : " + version);
                logger.Information("**********************************************************************************");

                if (ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().IsWaferlessMode)
                {
                    logger.Information("******************************************************************************************");
                    logger.Information(" WAFER LESS MODE ");
                    logger.Information("******************************************************************************************");
                }

                if (!string.IsNullOrEmpty(trace))
                {
                    logger.Error($"Current dir Modification has failed\n{trace}");
                    logger.Warning($"exe path = {exeName}");
                    logger.Warning($"working dir = {CurDir}");
                }

                if (true == CheckIncompatibleRunningInstances(logger))
                {
                    logger.Error("ANALYSE server is terminated abnormally due to incompatible already running Instance");
                    Environment.Exit(1); //Exit abnormally           
                }

                if (!Environment.UserInteractive)
                {
                    RunAsAService(logger);
                }
                else
                {
                    RunAsAConsole(logger);
                }
            }
            catch (Exception ex)
            {
                WriteErrorFile();
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
                ClassLocator.Default.GetInstance<ILogger<object>>()?.Information("ANALYSE server is terminated abnormally");
                ClassLocator.Default.GetInstance<ILogger<object>>()?.Error($"{ex.Message}\n### StackTrace :\n{ex.StackTrace}\n####");
                Environment.Exit(2); // exit abnormally
            }
        }

        private static void RunAsAConsole(ILogger logger)
        {
            if (!SetConsoleCtrlHandler(s_handler, true))
            {
                logger.Information("Unable to install console event handler.");
            }

            Console.WriteLine(".");
            try
            {
                logger.Information("----------------------------------");
                logger.Information("Start ANALYSE Console server ");

                logger.Verbose("      => Verbose logging activated");
                logger.Debug("      => Debug logging activated");
                Console.WriteLine(".");

                var anaServer = ClassLocator.Default.GetInstance<AnaServer>();
                anaServer.Start();

                while (true)
                {
                    Console.WriteLine("Press Ctrl-C to stop Analyse Server");
                    Console.ReadLine(); // InterruptReadLine exit
                }
            }
            catch (Exception ex)
            {
                WriteErrorFile();
                logger.Error(ex, "Analyse console error");
                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
                var anaServer = ClassLocator.Default.GetInstance<AnaServer>();
                anaServer?.Stop();
                Environment.Exit(3); // exit abnormally
            }
        }

        private static void RunAsAService(ILogger logger)
        {
            logger.Information("----------------------------------");
            logger.Information("Start ANALYSE Windows service");

            logger.Verbose("      => Verbose logging activated");
            logger.Debug("      => Debug logging activated");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AnaWindowsService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        public static bool CheckIncompatibleRunningInstances(ILogger logger)
        {
            bool bIsRunning = false;
            var thisProcess = Process.GetCurrentProcess();
            var sameprocesses = Process.GetProcessesByName(thisProcess.ProcessName);
            foreach (var sameProcess in sameprocesses)
            {
                if (sameProcess.Id != thisProcess.Id)
                {
                    bIsRunning = true;
                    logger.Error($"The Same {thisProcess.ProcessName} process is already running.");
                }
            }

            var forbiddenProcess = new string[] { "FPMS", "CamFlow", "CamServer" };
            foreach (var PName in forbiddenProcess)
            {
                var processes = Process.GetProcessesByName(PName);
                if (processes.Any())
                {
                    bIsRunning = true;
                    logger.Error($"A {PName} process is already running.");
                }
            }

            try
            {
                ServiceController service = new ServiceController(AnaServiceInstaller.ServiceName);
                if (!(service.Status.Equals(ServiceControllerStatus.Stopped)) &&
                    !(service.Status.Equals(ServiceControllerStatus.StopPending)))
                {
                    bIsRunning = true;
                    logger.Error($"<{AnaServiceInstaller.ServiceName}> service is already running.");
                }
            }
            catch (Exception)
            {
                logger.Warning($"Service <{AnaServiceInstaller.ServiceName}> is not install on this machine. Don't care of this warning if you are running service as a Console.");
            }
            if (bIsRunning)
            {
                logger.Fatal(null, "#################################");
                logger.Fatal(null, $"Analyse Server start up aborted.");
                logger.Fatal(null, "#################################");
                if (Environment.UserInteractive)
                {
                    var saveBack = Console.BackgroundColor;
                    var saveFront = Console.ForegroundColor;
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("###");
                    Console.WriteLine("     At least one incompatible process is running.\n     Terminate thoses processes and restart analyse server.");
                    Console.WriteLine("###");
                    Console.BackgroundColor = saveBack;
                    Console.ForegroundColor = saveFront;

                    Console.WriteLine("Press enter to exit");
                    Console.ReadLine();
                }
            }
            return bIsRunning;
        }
    }
}
