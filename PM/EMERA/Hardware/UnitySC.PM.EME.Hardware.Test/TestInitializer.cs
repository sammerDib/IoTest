using System;
using System.IO;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Hardware.Test
{
    [TestClass]
    public static class TestServices
    {
        [AssemblyInitialize]
        public static void InitializeBeforeAll(TestContext context)
        {

            var container = new Container();
            container.Options.EnableAutoVerification = false;
            ClassLocator.ExternalInit(container, true);
            ClassLocator.Default.Register(typeof(IMotionAxesServiceCallbackProxy), typeof(StubMotionAxesServiceCallbackProxy), true);
            ClassLocator.Default.Register<IEMEServiceConfigurationManager>(()=> new FakeConfigurationManager("ALPHA", null, true));
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>(), true);
            SerilogInit.Init(Path.Combine(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "ALPHA/Configuration/log.config"));

            var mockLogger = Mock.Of<IHardwareLogger>();
            var mockLoggerFactory = Mock.Of<IHardwareLoggerFactory>(x => x.CreateHardwareLogger(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) == mockLogger);
            ClassLocator.Default.Register<IHardwareLoggerFactory>(() => mockLoggerFactory);

            var configManager = new FakeConfigurationManager("ALPHA", null, true);
            ClassLocator.Default.Register(() => new CalibrationManager(configManager.CalibrationFolderPath, true), true);


        }
    }
}
