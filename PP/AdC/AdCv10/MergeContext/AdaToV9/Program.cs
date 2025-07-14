using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;

using AdaToAdc;

using ADCEngine;

using Serilog;

using UnitySC.PP.Shared.Configuration;
using UnitySC.Shared.Tools;

namespace ConsoleAdcToAdc
{
    public class Program
    {
        private static string _version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string Version { get { return _version; } }

        private static void Main(string[] args)
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture; // new System.Globalization.CultureInfo("en-US"); 

            Bootstrapper.Register();
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
            Log.Information("\n\nAdaToAdc version " + Version + " starting...\n\n");

            try
            {
                var adaToAdcService = new AdaToAdcService();

                // Démarrage en mode console
                if (Environment.UserInteractive)
                {
                    Log.Information("\n\n Console starting...\n\n");
                    PrintLogo();

                    //-------------------------------------------------------------
                    // Démarrage du scan des ADA
                    //-------------------------------------------------------------
                    string engineType = ConfigurationManager.AppSettings["AdcEngine.ProductionMode"];
                    if (engineType == "InADC")
                    {
                        Console.WriteLine("Press enter to start");
                        Console.ReadLine();
                    }

                    AppParameter.Instance.Init(
                   (p) =>
                   {

                       switch (p)
                       {
                          case "PathModuleDll":
                               return PathString.GetExecutingAssemblyPath().Directory;
                           case "PathUIResourcesXml":
                               return PathString.GetExecutingAssemblyPath().Directory;

                       }

                       return null;


                   }
                   );


                    adaToAdcService.Start(args);

                    //-------------------------------------------------------------
                    // Running
                    //-------------------------------------------------------------
                    Console.WriteLine("Press enter to stop");
                    Console.ReadLine();

                    //-------------------------------------------------------------
                    // Shutdown
                    //-------------------------------------------------------------
                    adaToAdcService.StopMonitor();
                    ADC.Instance.Shutdown();
                }
                // Démarrage en mode windows service
                else
                {
                    Log.Information("\n\n Windows service Init...\n\n");
                    Directory.SetCurrentDirectory(PathString.GetAppBaseDirectory());
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[]
                    {
                    adaToAdcService
                    };
                    ServiceBase.Run(adaToAdcService);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private static void PrintLogo()
        {
            Console.WriteLine(@"           , ''':                              ");
            Console.WriteLine(@"         '''''''''''                           ");
            Console.WriteLine(@"       :'':        :''                         ");
            Console.WriteLine(@"      ''`             '                        ");
            Console.WriteLine(@"     '.                ,                       ");
            Console.WriteLine(@"    '`                     `'                  ");
            Console.WriteLine(@"   '                       '';                 ");
            Console.WriteLine(@"  .`                       ''  '',             ");
            Console.WriteLine(@"  '                            ''`             ");
            Console.WriteLine(@" ,    ''      ;''''''''.   '' ''''''''      '';");
            Console.WriteLine(@" '   `''      ''''''''''. ;'' ''''''''      ';`");
            Console.WriteLine(@" ,   ;''      ''''    ''' ''' ,''           '' ");
            Console.WriteLine(@".`   '''      '''     :'' ''' '''  '';      '' ");
            Console.WriteLine(@"'    '''      ''      ,'' '', '''  ''`     `'' ");
            Console.WriteLine(@"'    '',     `';      ''' ''` '''  ''      :'' ");
            Console.WriteLine(@"'`   '':     '''      ''' ''  ''.  ''`     ''' ");
            Console.WriteLine(@"'.   '''    ;'''      '';`''  ''`  '''    '''' ");
            Console.WriteLine(@"''   '''''''''''      ''.,''' '''; :'''''''''; ");
            Console.WriteLine(@"`'    ''''''''':      ''` ''' ''',  :''''''''` ");
            Console.WriteLine(@" ',     ```  ;;`      ;;  `;`   :`     ,`  ''  ");
            Console.WriteLine(@" ''                                        ''  ");
            Console.WriteLine(@"  ''                                      '''  ");
            Console.WriteLine(@"  '''                   '              '''''   ");
            Console.WriteLine(@"   '''                 '              '''''    ");
            Console.WriteLine(@"    '''              ''               ..`      ");
            Console.WriteLine(@"     ''''`         '';                         ");
            Console.WriteLine(@"      `'''''';''''''                           ");
            Console.WriteLine(@"         ;''''''',                             ");
            Console.WriteLine(@"                                               ");
        }

    }
}
