using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using CommunityToolkit.Mvvm.Messaging;

using Moq;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;
using System.Linq;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.PM.ANA.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.Shared;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;

namespace UnitySC.PM.ANA.Client.Proxy.Test
{
    [TestClass]
    public class Initialize
    {
        #region Fields

        private static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).AbsolutePath).Replace("%20", " ");
        }

        protected static Mock<IDialogOwnerService> dialogServiceMock = new Mock<IDialogOwnerService>();

        #endregion Fields

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Console.WriteLine("AssemblyInitialize");

            // we check if the server host is already running
            var serviceHostProcess = Process.GetProcessesByName("UnitySC.PM.ANA.Service.Host");

            if (serviceHostProcess.Length == 0 || serviceHostProcess.FirstOrDefault().SessionId != Process.GetCurrentProcess().SessionId)
            {
                // Start service host
                var serverHost = new Process();
                string serviceHostExeName = "UnitySC.PM.ANA.Service.Host.exe";
#if USE_ANYCPU
                string baseDirectory = Path.GetFullPath(Path.Combine(GetCurrentDirectory(), @"..\..\..\..\"));
                string serviceHostDebugFolder = Path.Combine(baseDirectory, @"Service\UnitySC.PM.ANA.Service.Host\bin\Debug");
                string serviceHostReleaseFolder = Path.Combine(baseDirectory, @"Service\UnitySC.PM.ANA.Service.Host\bin\Release");
#else
                string baseDirectory = Path.GetFullPath(Path.Combine(GetCurrentDirectory(), @"..\..\..\..\..\"));
                string serviceHostDebugFolder = Path.Combine(baseDirectory, @"Service\UnitySC.PM.ANA.Service.Host\bin\x64\Debug");
                string serviceHostReleaseFolder = Path.Combine(baseDirectory, @"Service\UnitySC.PM.ANA.Service.Host\bin\x64\Release");
#endif

                // We look for the service host in the debug folder
                if (File.Exists(Path.Combine(serviceHostDebugFolder, serviceHostExeName)))
                {
                    serverHost.StartInfo.FileName = Path.Combine(serviceHostDebugFolder, serviceHostExeName);
                    serverHost.StartInfo.WorkingDirectory = serviceHostDebugFolder;
                }
                else
                {
                    // We look for the service host in the release folder
                    if (File.Exists(Path.Combine(serviceHostReleaseFolder, serviceHostExeName)))
                    {
                        serverHost.StartInfo.FileName = Path.Combine(serviceHostReleaseFolder, serviceHostExeName);
                        serverHost.StartInfo.WorkingDirectory = serviceHostReleaseFolder;
                    }
                    else
                        throw (new Exception("Unable to execute the service host"));
                }
                serverHost.StartInfo.Arguments = "-c 4MET2229 -sh -sf -rf";
                serverHost.StartInfo.UseShellExecute = true;
                serverHost.Start();
            }

            // Configuration
            var currentConfiguration = new FakeConfigurationManager();

            // Init logger
            SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => currentConfiguration, true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            // Message
            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);

            // Dialog service
            ClassLocator.Default.Register<IDialogOwnerService>(() => dialogServiceMock.Object);

            ClassLocator.Default.Register<IProbesFactory, ProbesVMFactory>(true);

            ClassLocator.Default.Register(() => new GlobalStatusSupervisor(ActorType.ANALYSE, false, ClassLocator.Default.GetInstance<ILogger<GlobalStatusSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>()), true);
            ClassLocator.Default.Register<IKeyboardMouseHook, KeyboardMouseHook.KeyboardMouseHook>(true);

            // We connect to the server it will then start initialization
            var globalStatusSupervisor = ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();

            globalStatusSupervisor.Init();

            var messenger = ClassLocator.Default.GetInstance<IMessenger>();

            // We wait for hardware initialization
            var resInit = globalStatusSupervisor.WaitHardwareInitializationDone();
            if (!resInit.Result)
                throw (new Exception("Hardware initialization failed"));
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("AssemblyCleanup");

            var serviceHostProcess = Process.GetProcessesByName("UnitySC.PM.ANA.Service.Host");
            if (serviceHostProcess.Length != 0)
                foreach (var process in serviceHostProcess)
                    if (process.SessionId == Process.GetCurrentProcess().SessionId)
                        process.Kill();
        }
    }
}
