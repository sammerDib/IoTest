using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Host
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            // Catch all unhandled exceptions, except StackOverFlow not thrown by user code
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);           
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
            string trace = null;
            string CurDir = Directory.GetCurrentDirectory();
            ILogger logger = null;
            try
            {
                string ExeDir = Path.GetDirectoryName(exeName);
                Directory.SetCurrentDirectory(ExeDir);
            }
            catch (Exception ex)
            {
                trace = ex.StackTrace;
                WriteErrorFile();
            }           

            try
            {
                Bootstrapper.Register(args);
                logger = ClassLocator.Default.GetInstance<ILogger<object>>();

                if (!string.IsNullOrEmpty(trace))
                {
                    logger?.Error($"Current dir Modification has failed\n{trace}");
                    logger?.Warning($"exe path = {exeName}");
                    logger?.Warning($"working dir = {CurDir}");
                }
                Initialize(logger);
                ApplicationRunner.Run(logger);
            }
            catch (Exception ex)
            {
                if (logger == null)
                {                    
                    logger = ClassLocator.Default.GetInstance<ILogger<object>>();
                }
                ErrorHandler.HandleGeneralException(ex, args, logger);                
            }           
        }       
        public static void Initialize(ILogger logger)
        {                   
            LogApplicationStart(logger);
        }
        public static void LogApplicationStart(ILogger logger)
        {
            var version = GetApplicationVersion();
            logger.Information("**********************************************************************************");
            logger.Information("  EMERA SERVER Version : " + version);
            logger.Information("**********************************************************************************");
        }
        private static string GetApplicationVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
        public static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            WriteErrorFile();
        }
        static void WriteErrorFile()
        {
            File.AppendAllLines(ConsoleSignalHandler.ErrorFilePath, new List<string>() { DateTime.UtcNow + "EMERA server fatal error." });
        }
    }
}
