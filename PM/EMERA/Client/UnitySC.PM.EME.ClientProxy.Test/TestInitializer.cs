using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.EME.Client.Shared;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Proxy.Test
{
    [TestClass]
    public static class TestInitializer
    {
        [AssemblyInitialize]
        public static void InitializeBeforeAll(TestContext context)
        {
            StartServerHost();
            ConfigureClient();
        }

        private static void ConfigureClient()
        {
            ClassLocator.ExternalInit(new Container(), true);
            string configFile = Path.Combine(GetCurrentDirectory(), "../../ALPHA/ClientConfiguration.xml");
            var configuration = new ClientConfigurationManager(null);
            ClassLocator.Default.Register<IClientConfigurationManager>(() => configuration, true);
            ClassLocator.Default.Register<ClientConfiguration>(() => EmeClientConfiguration.Init(configFile), true);            

            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
            ClassLocator.Default.Register<AlgoSupervisor>();
            ClassLocator.Default.Register<IKeyboardMouseHook, KeyboardMouseHook.KeyboardMouseHook>(true);
        }

        private static void StartServerHost()
        {
            if (!IsServerHostAlreadyRunning())
            {
                return;
            }

            var serverHost = new Process();
            string baseDirectory = Path.GetFullPath(Path.Combine(GetCurrentDirectory(), @"..\..\..\..\..\"));
            string serviceHostDebugFolder =
                Path.Combine(baseDirectory, @"Service\UnitySC.PM.EME.Service.Host\bin\x64\Debug");
            string serviceHostReleaseFolder =
                Path.Combine(baseDirectory, @"Service\UnitySC.PM.EME.Service.Host\bin\x64\Release");

            if (File.Exists(Path.Combine(serviceHostDebugFolder, "UnitySC.PM.EME.Service.Host.exe")))
            {
                serverHost.StartInfo.FileName = Path.Combine(serviceHostDebugFolder, "UnitySC.PM.EME.Service.Host.exe");
                serverHost.StartInfo.WorkingDirectory = serviceHostDebugFolder;
            }
            else if (File.Exists(Path.Combine(serviceHostReleaseFolder, "UnitySC.PM.EME.Service.Host.exe")))
            {
                serverHost.StartInfo.FileName =
                    Path.Combine(serviceHostReleaseFolder, "UnitySC.PM.EME.Service.Host.exe");
                serverHost.StartInfo.WorkingDirectory = serviceHostReleaseFolder;
            }
            else
            {
                throw new Exception("Unable to execute the service host");
            }

            serverHost.StartInfo.Arguments = "-c ALPHA -sh -sf -rf";
            serverHost.StartInfo.UseShellExecute = false;
            serverHost.StartInfo.RedirectStandardOutput = true;
            serverHost.Start();

            WaitForHardwareInitialization(serverHost);
        }

        private static void WaitForHardwareInitialization(Process server)
        {
            int count = 0;
            while (count < 100)
            {
                string line = server.StandardOutput.ReadLine();
                if (line != null && line.Contains("Set global status State:Free"))
                {
                    break;
                }

                if (!server.StandardOutput.EndOfStream)
                {
                    continue;
                }

                Thread.Sleep(100);
                count++;
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            var serviceHostProcess = Process.GetProcessesByName("UnitySC.PM.EME.Service.Host");
            if (serviceHostProcess.Length == 0)
                return;
            foreach (var process in serviceHostProcess.Where(process =>
                         process.SessionId == Process.GetCurrentProcess().SessionId))
            {
                process.Kill();
            }
        }

        private static bool IsServerHostAlreadyRunning()
        {
            var serviceHostProcess = Process.GetProcessesByName("UnitySC.PM.EME.Service.Host");
            return serviceHostProcess.Length == 0 || serviceHostProcess.FirstOrDefault()?.SessionId !=
                Process.GetCurrentProcess().SessionId;
        }

        private static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).AbsolutePath)?
                .Replace("%20", " ");
        }
    }
}
