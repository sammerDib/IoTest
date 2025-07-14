using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Service.Host
{
    public static class ErrorHandler
    {
        public static void HandleConsoleError(Exception ex, ILogger logger)
        {
            WriteErrorFile();
            logger?.Error(ex, "EMERA console error");
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();
            Environment.Exit(3);
        }

        public static void HandleGeneralException(Exception ex, string[] args, ILogger logger)
        {
            WriteErrorFile();
            if (Environment.UserInteractive)
            {
                var saveBack = Console.BackgroundColor;
                var saveFront = Console.ForegroundColor;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("###");
                Console.WriteLine("Initialisation error with args " + string.Concat(args.Skip(1).ToArray()));
                Console.WriteLine(ex.ToString());
                Console.WriteLine("###");
                Console.BackgroundColor = saveBack;
                Console.ForegroundColor = saveFront;

                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
            logger?.Information("EMERA server is terminated abnormally");
            logger?.Error(ex, $"{ex.Message}\n### StackTrace :\n{ex.StackTrace}\n####");
            Environment.Exit(1); // exit abnormally     
        }
        private static void WriteErrorFile()
        {
            File.AppendAllLines(ConsoleSignalHandler.ErrorFilePath, new List<string>() { DateTime.UtcNow + " EMERA server fatal error." });
        }
    }
}
