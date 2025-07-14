using System;

using CommunityToolkit.Mvvm.Messaging;

using Moq;

using SimpleInjector;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Implementation;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Implementation.Execution;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.PM.DMT.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.MonitorTasks;

namespace UnitySC.PM.DMT.Service.Test
{
    public static class Bootstrapper
    {
        public static object BootstrapperLock = new object();
        public static bool IsRegister { get; private set; }

        public static void Register(Container container)
        {
            lock (BootstrapperLock)
            {
                if (!IsRegister)
                {
                    // Configuration
                    var currentConfiguration = new DMTFakeConfigurationManager();

                    ClassLocator.ExternalInit(container, true);

                    // Init logger
                    SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);
                    ClassLocator.Default.Register<DMTFakeConfigurationManager>(() => currentConfiguration, true);
                    ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => currentConfiguration, true);
                    ClassLocator.Default.Register<IDMTServiceConfigurationManager>(() => currentConfiguration, true);

                    // Logger with caller name
                    ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
                    var mockLogger = Mock.Of<IHardwareLogger>();
                    var mockLoggerFactory = Mock.Of<IHardwareLoggerFactory>(x => x.CreateHardwareLogger(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) == mockLogger);
                    ClassLocator.Default.Register<IHardwareLoggerFactory>(() => mockLoggerFactory);

                    // Logger without caller name
                    ClassLocator.Default.Register<ILogger, SerilogLogger<object>>();
                    ClassLocator.Default.Register<ClassLogWithCaller>();
                    ClassLocator.Default.Register<ClassLogWithoutCaller>();

                    // Message
                    ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);

                    // Global status service
                    ClassLocator.Default.Register<GlobalStatusService>(true);
                    ClassLocator.Default.Register<IGlobalStatusServer>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());
                    ClassLocator.Default.Register<IGlobalStatusService>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());

                    // Demeter Recipe Services
                    ClassLocator.Default.Register<RecipeExecution>(true);
                    ClassLocator.Default.Register<DMTRecipeService>(true);
                    ClassLocator.Default.Register<IDMTRecipeService>(() => ClassLocator.Default.GetInstance<DMTRecipeService>(), true);

                    // Axes service
                    ClassLocator.Default.Register<IAxesService, AxesService>(true);

                    // Axes service CallBack Proxy
                    ClassLocator.Default.Register<IAxesServiceCallbackProxy, AxesService>(true);

                    // Chamber service
                    ClassLocator.Default.Register<IChamberService, ChamberService>(true);

                    // Chuck service
                    ClassLocator.Default.Register<IChuckService, ChuckService>(true);

                    // Camera manager
                    ClassLocator.Default.Register<IDMTInternalCameraMethods>(() => ClassLocator.Default.GetInstance<DMTCameraManager>(), true);

                    // Algorithm manager
                    ClassLocator.Default.Register<AlgorithmManager>(true);

                    // Screen service
                    ClassLocator.Default.Register<IDMTScreenService, DMTScreenService>(true);

                    // Calibration Manager
                    ClassLocator.Default.Register<CalibrationManager>(true);
                    ClassLocator.Default.Register<ICalibrationManager>(() => ClassLocator.Default.GetInstance<CalibrationManager>(), true);

                    // Flows configuration
                    ClassLocator.Default.Register(() => FlowsConfiguration.Init(currentConfiguration), true);
                    ClassLocator.Default.Register<IFlowsConfiguration>(() => ClassLocator.Default.GetInstance<FlowsConfiguration>(), true);

                    // Measures configuration
                    ClassLocator.Default.Register(() => MeasuresConfiguration.Init(currentConfiguration), true);

                    //Monitor Task timer
                    ClassLocator.Default.Register<MonitorTaskTimer>(true);
                    ClassLocator.Default.Register(() => new Lazy<MonitorTaskTimer>(ClassLocator.Default.GetInstance<MonitorTaskTimer>));
                    
                    IsRegister = true;
                }
            }
        }
    }
}
