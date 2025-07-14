using System;
using System.Configuration;

using Serilog;

using UnitySC.Shared.Tools;


namespace AdcToRobot
{
    public class Program
    {
        private static string _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string Version { get { return _version; } }

        private static void Main(string[] args)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture; // new System.Globalization.CultureInfo("en-US"); 

            PathString logfolder = ConfigurationManager.AppSettings["LogFolder"];
            PathString logfile = PathString.GetExeFullPath().ChangeExtension(".log").Filename;
            Log.Logger = new LoggerConfiguration()
                               .ReadFrom.AppSettings()
                               .WriteTo.File(path: logfolder / logfile,
                                            rollOnFileSizeLimit: true,
                                            fileSizeLimitBytes: 20971520,
                                            retainedFileCountLimit: 100)
                               .WriteTo.Console()
                               .CreateLogger();
            Log.Information("\n\nAdc To Robot: VID transfer tool, version " + Version + " starting...\n\n");

            try
            {
                AdcToRobot adcToRobot = new AdcToRobot();
                adcToRobot.Start();

                Console.WriteLine("Press enter to stop");
                Console.WriteLine();
                Console.ReadLine();

                adcToRobot.Stop();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

    }
}
