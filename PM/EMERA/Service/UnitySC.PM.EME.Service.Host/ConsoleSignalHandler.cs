using System;
using System.Runtime.InteropServices;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Host
{
    public static class ConsoleSignalHandler
    {
        // See https://learn.microsoft.com/en-us/windows/console/handlerroutine
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);
        private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);
        private static readonly SetConsoleCtrlEventHandler s_handler = Handler; // Keep a reference on delegate to avoid garbage collection

        public const string ErrorFilePath = "errorAppCrash.txt";
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

        public static void InstallHandler(ILogger logger)
        {
            if (!SetConsoleCtrlHandler(s_handler, true))
            {
                logger.Warning("Unable to install console event handler.");
            }
        }

        private static bool Handler(CtrlType signal)
        {
            ILogger logger = ClassLocator.Default.GetInstance<ILogger<object>>();
            var emeServer = ClassLocator.Default.GetInstance<EmeServer>();
            switch (signal)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                    TerminateServer(emeServer, signal, logger);
                    return false;

                default:
                    logger.Warning("Signal not handled");
                    break;
            }
            return false;
        }

        private static void TerminateServer(EmeServer emeServer, CtrlType signal, ILogger logger)
        {
            InterruptReadLine();

            logger.Information(signal.ToString() + " signal received. Closing EMERA server.");
            emeServer?.Stop();
            logger.Information(signal.ToString() + " EMERA server successfully stopped.");
            Environment.Exit(0);
        }
    }
}
