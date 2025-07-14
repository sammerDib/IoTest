using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;

using UnitySC.DataAccess.Service.Implementation;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Host
{
    internal class Program
    {
        private static readonly string s_exePath = Assembly.GetExecutingAssembly().Location;
        private const string DataAccessConfigurationFileName = "DataAccessConfiguration.xml";
        private const string FDCsConfigurationFileName = "FDCsConfiguration.xml";
        private const string FDCsPersitentDataFileName = "FDCsPersistentData.fpd";


        /// <summary>
        /// Host Service as console or as Windows service.
        /// </summary>
        private static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            string configurationRootFolder = ".";
           

            

            if (args != null && args.Length > 1)
            {
                if (args[0] == "-c")
                {
                    // args[1] contains the root folder of the configuration
                    configurationRootFolder=args[1];
                }
            }
            
            
            DataAccessConfiguration.SettingsFilePath = Path.Combine(configurationRootFolder,DataAccessConfigurationFileName);

            string fDCsConfigurationFilePath = Path.Combine(configurationRootFolder, FDCsConfigurationFileName);
            string fDCsPersistentDataFilePath = Path.Combine(configurationRootFolder, FDCsPersitentDataFileName);

            //FDC Manager
            ClassLocator.Default.Register(() => new FDCManager(fDCsConfigurationFilePath, fDCsPersistentDataFilePath), true);

            // Run as a service is not used for the moment. It could be used later

            //if (args != null && args.Length > 0)
            //{
            //    bool PauseConsole = false;
            //    if (!IsAdmin())
            //    {
            //        // Restart program and run as admin
            //        ProcessStartInfo startInfo = new ProcessStartInfo(s_exePath);
            //        startInfo.UseShellExecute = true;
            //        startInfo.Arguments = args[0];
            //        startInfo.Verb = "runas";
            //        startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //        System.Diagnostics.Process.Start(startInfo);
            //        Environment.Exit(0);
            //    }

            //    try
            //    {
            //        Console.WriteLine($"  DataAccess SERVER Version : " + version);
            //        Console.WriteLine($" ---------------------------------------");
            //        ServiceController service = new ServiceController(DataAccessServiceInstaller.ServiceName);
            //        if (args[0].Equals("-start", StringComparison.OrdinalIgnoreCase))
            //        {
            //            if ((service.Status.Equals(ServiceControllerStatus.Stopped)) ||
            //                (service.Status.Equals(ServiceControllerStatus.StopPending)))
            //            {
            //                Console.WriteLine($" Please Wait, Service is Starting...");
            //                service.Start();

            //                service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 40));
            //                Console.WriteLine($" Service Started");
            //            }
            //            else
            //            {
            //                Console.WriteLine($" Service already Started");
            //            }
            //        }
            //        else if (args[0].Equals("-stop", StringComparison.OrdinalIgnoreCase))
            //        {
            //            if (!(service.Status.Equals(ServiceControllerStatus.Stopped)) &&
            //                !(service.Status.Equals(ServiceControllerStatus.StopPending)))
            //            {
            //                Console.WriteLine($" Please Wait, Service is Stopping...");
            //                service.Stop();
            //                service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 40));
            //                Console.WriteLine($" Service Stopped");
            //            }
            //            else
            //            {
            //                Console.WriteLine($" Service already Stopped");
            //            }
            //        }
            //        else
            //        {
            //            Console.WriteLine("Invalid argument!");
            //        }
            //    }
            //    catch (System.ServiceProcess.TimeoutException toex)
            //    {
            //        string msg = toex.Message;
            //        Console.WriteLine($"# ERROR : Service <{DataAccessServiceInstaller.ServiceName}> Timeout");
            //        Console.WriteLine(msg);
            //        PauseConsole = true;
            //    }
            //    catch (InvalidOperationException opex)
            //    {
            //        string msg = opex.Message;
            //        Console.WriteLine($"# ERROR : Service <{DataAccessServiceInstaller.ServiceName}> has not been found\nCheck it has been correctly installed and set in your Windows Services ");
            //        PauseConsole = true;
            //    }
            //    catch (Exception e)
            //    {
            //        string msg = e.Message;
            //        Console.WriteLine($"# ERROR : Unknow exception on Service <{DataAccessServiceInstaller.ServiceName}>");
            //        Console.WriteLine(msg);
            //        PauseConsole = true;
            //    }
            //    finally
            //    {
            //        Console.WriteLine("-------------------");
            //        if (PauseConsole)
            //        {
            //            Console.WriteLine("Press enter to quit");
            //            Console.ReadLine();
            //        }
            //    }
            //}
            //else
            {
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

                Bootstrapper.Register();
                ILogger _logger = ClassLocator.Default.GetInstance<ILogger<object>>();

                _logger.Information("**********************************************");
                _logger.Information("  DataAccess SERVER Version : " + version);
                _logger.Information("**********************************************");

                if (!string.IsNullOrEmpty(trace))
                {
                    _logger.Error($"Current dir Modification has failed\n{trace}");
                    _logger.Warning($"exe path = {exeName}");
                    _logger.Warning($"working dir = {CurDir}");
                }

               
                ClassLocator.Default.GetInstance<ApplicationFDCs>().Register();

                if ((!Environment.UserInteractive))
                {
                    Program.RunAsAService(_logger);
                }
                else
                {
                    Program.RunAsAConsole(_logger);
                }
            }
        }

        internal static bool IsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void RunAsAConsole(ILogger logger)
        {
            Console.WriteLine(".");
            try
            {
                // Check if service not already running in a "hidden way"
                try
                {
                    ServiceController sc = new ServiceController(DataAccessServiceInstaller.ServiceName);
                    if (sc.Status != ServiceControllerStatus.Stopped)
                    {
                        logger.Error(" DataAccess Service is currently RUNNING");
                        logger.Error($"** Please Stop <{DataAccessServiceInstaller.ServiceName}> before launch DataAccess as console **\n");
                        throw new Exception("DataAccess service already running");
                    }
                }
                catch (ArgumentException) { }       // only in case of service not installed (According to doc not seen)
                catch (InvalidOperationException)   // only in case of service not installed
                {
                    // service name is invalid
                    // this service is not installed on this machine
                }

                logger.Information("----------------------------------");
                logger.Information("Start DataAccess Console serveur ");

                logger.Verbose("      => Verbose logging activated");
                logger.Debug("      => Debug logging activated");
                Console.WriteLine(".");

                var _dataAccessService = ClassLocator.Default.GetInstance<DataAccessService>();
                _dataAccessService.Start();

                Console.WriteLine("Press enter to stop");
                Console.ReadLine();
                _dataAccessService.Stop();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DataAccess console error");
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
            logger.Information("Start DataAccess Windows service");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                    new DataAccessWindowsService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
