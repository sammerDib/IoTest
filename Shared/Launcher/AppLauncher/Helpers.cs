using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Media;

using AppLauncher.ViewModel;

using UnitySC.Shared.UI.Helper;

namespace AppLauncher
{
    public static class Helpers
    {

        internal const int CTRL_C_EVENT = 0;
        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);

        // Delegate type to be used as the Handler Routine for SCCH
        delegate Boolean ConsoleCtrlDelegate(uint CtrlType);

        public static void StartApplication(string path, string arguments, bool hideConsole)
        {
            // Create a new ProcessStartInfo object and set the FileName property to the path of the console application.
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = path,
                Arguments = arguments,
                UseShellExecute = hideConsole ? false : true,             // Set to false to hide the console window.
                CreateNoWindow = hideConsole ? true : false,              // Set to true to hide the console window.
                RedirectStandardOutput = false,     // Set to true if you want to capture the console application's output.
                WorkingDirectory = Path.GetDirectoryName(path)
            };

            // Start the process.
            Process process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();

            // Optionally, you can capture the output of the console application and do something with it.
            //string output = process.StandardOutput.ReadToEnd();
            //process.WaitForExit();
        }

        public static void StopApplication(string applicationName)
        {
            Process[] processes = Process.GetProcessesByName(applicationName);

            if (processes.Length > 0)
            {
                // Assuming there's only one instance of the process, we use the first one in the array
                Process process = processes[0];

                if (!process.HasExited)
                {
                    if (!gracefullyShutdown(process))
                    {
                        process.Kill();
                    }

                    Console.WriteLine($"The {applicationName} application has been stopped.");
                }
                else
                {
                    Console.WriteLine($"The {applicationName} application is already not running.");
                }
            }
            else
            {
                Console.WriteLine($"The {applicationName} application is not currently running.");
            }
        }

        private static bool gracefullyShutdown(Process p)
        {
            if (AttachConsole((uint)p.Id))
            {
                SetConsoleCtrlHandler(null, true);
                try
                {
                    if (!GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0))
                    {
                        return false;
                    }

                    if (!p.WaitForExit(30_000))
                    {
                        return false;
                    }
                }
                finally
                {
                    SetConsoleCtrlHandler(null, false);
                    FreeConsole();
                }
                return true;
            }
            return false;
        }

        public static ExecutionStatus GetApplicationRunningStatus(string applicationName)
        {
            if (Process.GetProcessesByName(applicationName).Any())
                return ExecutionStatus.Running;

            return ExecutionStatus.Stopped;
        }


        public static void StartWindowsService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));
        }

        public static void StopWindowsService(string serviceName)
        {
            ServiceController service = new ServiceController(serviceName);
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 30));
        }


        public static ExecutionStatus GetServiceRunningStatus(string serviceName)
        {
            ExecutionStatus status = ExecutionStatus.Unknown;
            try
            {
                ServiceController service = new ServiceController(serviceName);
                service.Refresh();
                switch (service.Status)
                {
                    case ServiceControllerStatus.StartPending:
                    case ServiceControllerStatus.Running:
                    case ServiceControllerStatus.ContinuePending:
                        status = ExecutionStatus.Running;
                        break;

                    case ServiceControllerStatus.StopPending:
                    case ServiceControllerStatus.Stopped:
                    case ServiceControllerStatus.PausePending:
                    case ServiceControllerStatus.Paused:
                        status = ExecutionStatus.Stopped;
                        break;

                    default:
                        status = ExecutionStatus.Unknown;
                        break;
                }
            }
            catch (Exception)
            {
                status = ExecutionStatus.Unknown;

            }
            return status;
        }



        public static ImageSource GetApplicationIcon(string applicationPath)
        {
            if (File.Exists(applicationPath))
            {
                Icon appIcon = Icon.ExtractAssociatedIcon(applicationPath);
                Bitmap bitmapIcon = appIcon.ToBitmap();

                if (appIcon != null)
                {
                    ImageSource iconImageSource = ImageHelper.ConvertToBitmapSource(bitmapIcon);
                    if (iconImageSource != null)
                    {
                        return iconImageSource;
                    }
                }

            }
            return null;
        }

        public static ImageSource GetServiceIcon(string serviceName)
        {

            ServiceController[] services = ServiceController.GetServices();
            ServiceController targetService = null;

            foreach (ServiceController service in services)
            {
                if (service.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
                {
                    targetService = service;
                    break;
                }
            }

            if (targetService != null)
            {
                try
                {
                    Icon serviceIcon = Icon.ExtractAssociatedIcon(targetService.DisplayName);
                    Bitmap bitmapIcon = serviceIcon.ToBitmap();

                    if (serviceIcon != null)
                    {
                        ImageSource iconImageSource = ImageHelper.ConvertToBitmapSource(bitmapIcon);
                        if (iconImageSource != null)
                        {
                            return iconImageSource;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Helpers catch Exception : {ex.Message}");
                    return null;
                }


            }
            return null;
        }


        public static string GetApplicationVersion(string filePath)
        {
            string productVersion;
            string fileVersion;
            try
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(filePath);

                string productName = versionInfo.ProductName;
                productVersion = versionInfo.ProductVersion;
                fileVersion = versionInfo.FileVersion;

                Console.WriteLine($"Product Name: {productName}");
                Console.WriteLine($"Product Version: {productVersion}");
                Console.WriteLine($"File Version: {fileVersion}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Helpers catch Exception : {ex.Message}");
                return String.Empty;
            }

            return fileVersion;
        }

        public static string GetServiceVersion(string serviceName)
        {
            string productVersion = string.Empty;
            string fileVersion = string.Empty;
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_Service WHERE Name = '{serviceName}'"))
                {
                    ManagementObject serviceObject = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

                    if (serviceObject != null)
                    {
                        string displayName = serviceObject["DisplayName"].ToString();
                        string servicePath = serviceObject["PathName"].ToString();

                        Console.WriteLine($"Service Display Name: {displayName}");
                        Console.WriteLine($"Service Path: {servicePath}");

                        // Extract version information from the service executable file
                        if (!string.IsNullOrEmpty(servicePath))
                        {
                            string serviceExecutablePath = servicePath.Split('"')[1];

                            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(serviceExecutablePath);

                            productVersion = versionInfo.ProductVersion;
                            fileVersion = versionInfo.FileVersion;

                        }
                        else
                        {
                            Console.WriteLine("Service executable path not found.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Service not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return fileVersion;
        }

    }
}





