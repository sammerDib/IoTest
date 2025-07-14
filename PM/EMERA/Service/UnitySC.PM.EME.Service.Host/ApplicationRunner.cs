using System;
using System.ServiceProcess;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Host
{
    internal static class ApplicationRunner
    {
        public static void Run(ILogger logger)
        {            
            if (!Environment.UserInteractive)
            {
                RunAsAService(logger);
            }
            else
            {
                RunAsAConsole(logger);
            }
        }
        private static void RunAsAConsole(ILogger logger)
        {
            ConsoleSignalHandler.InstallHandler(logger);

            try
            {
                var emeServer = ClassLocator.Default.GetInstance<EmeServer>();
                emeServer.Start();

                while (true)
                {
                    Console.WriteLine("Press Ctrl-C to stop EMERA Server");
                    Console.ReadLine();                   
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleConsoleError(ex, logger);
            }
            finally
            {
                Console.WriteLine("Press enter to quit");
                Console.ReadLine();
            }
        }

        private static void RunAsAService(ILogger logger)
        {
            logger.Information("----------------------------------");
            logger.Information("Start EMERA Windows service");

            logger.Verbose("      => Verbose logging activated");
            logger.Debug("      => Debug logging activated");
            ServiceBase[] ServicesToRun = new ServiceBase[] { new EmeWindowsService() };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
