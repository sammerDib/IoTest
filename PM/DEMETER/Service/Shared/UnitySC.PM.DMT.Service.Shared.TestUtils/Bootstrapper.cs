using System;

using CommunityToolkit.Mvvm.Messaging;

using Moq;

using SimpleInjector;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.DMTContextApplier;
using UnitySC.PM.DMT.Service.Implementation;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Implementation.Execution;
using UnitySC.PM.DMT.Service.Implementation.Fringes;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.AlgorithmManager;
using UnitySC.PM.DMT.Service.Interface.Calibration;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.PM.DMT.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Dataflow.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Implementation;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Implementation;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Implementation;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.MonitorTasks;

namespace UnitySC.PM.DMT.Service.Shared.TestUtils
{
    public static class Bootstrapper
    {
        public static Mock<DMTCameraManager> SimulatedCameraManager;

        public static Mock<IDMTCameraService> SimulatedCameraService;

        public static void Register(Container container, bool flowsAreSimulated = false, bool milIsSimulated = true)
        {
            var configurationManager = new DMTFakeConfigurationManager(flowsAreSimulated, milIsSimulated);

            // Init logger
            SerilogInit.Init(configurationManager.LogConfigurationFilePath);
            ClassLocator.ExternalInit(container, true);

            // Service configuration
            ClassLocator.Default.Register<IServiceConfigurationManager>(() => configurationManager, true);
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => configurationManager, true);
            ClassLocator.Default.Register<IDMTServiceConfigurationManager>(() => configurationManager, true);

            // Logger registration
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register<ILogger, SerilogLogger<object>>();
            var mockLogger = Mock.Of<IHardwareLogger>();
            var mockLoggerFactory = Mock.Of<IHardwareLoggerFactory>(x => x.CreateHardwareLogger(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) == mockLogger);
            ClassLocator.Default.Register<IHardwareLoggerFactory>(() => mockLoggerFactory);

            // PM Configuration
            ClassLocator.Default.Register(() => PMConfiguration.Init(configurationManager.PMConfigurationFilePath), true);
            ClassLocator.Default.Register<ModuleConfiguration>(() => ClassLocator.Default.GetInstance<PMConfiguration>(), true);

            // Flows configuration
            ClassLocator.Default.Register(() => FlowsConfiguration.Init(configurationManager), true);
            ClassLocator.Default.Register<IFlowsConfiguration>(() => ClassLocator.Default.GetInstance<FlowsConfiguration>(),
                true);

            // Measures configuration
            ClassLocator.Default.Register(() => MeasuresConfiguration.Init(configurationManager), true);

            // Flow initial context applier
            ClassLocator.Default.Register(typeof(ContextApplier<>), typeof(DMTContextApplier<>));

            //Messenger
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>();

            // Global status
            ClassLocator.Default.Register<IGlobalStatusServer, GlobalStatusService>(true);
            ClassLocator.Default.Register<IGlobalStatusService, GlobalStatusService>(true);

            // Hardware manager
            ClassLocator.Default.Register<DMTHardwareManager>(true);
            ClassLocator.Default.Register<HardwareManager>(() => ClassLocator.Default.GetInstance<DMTHardwareManager>(), true);
            ClassLocator.Default.Register<IHardwareManager>(() => ClassLocator.Default.GetInstance<DMTHardwareManager>(), true);

            // Calibration
            ClassLocator.Default.Register<DMTCalibrationService>(true);
            ClassLocator.Default.Register<ICalibrationService>(() => ClassLocator.Default.GetInstance<DMTCalibrationService>(), true);
            ClassLocator.Default.Register<CalibrationManager>(true);
            ClassLocator.Default.Register<ICalibrationManager>(() => ClassLocator.Default.GetInstance<CalibrationManager>(), true);
            
            // AlgorithmService
            ClassLocator.Default.Register<AlgorithmManager>(true);
            ClassLocator.Default.Register<IDMTAlgorithmManager>(() => ClassLocator.Default.GetInstance<AlgorithmManager>(), true);
            ClassLocator.Default.Register<DMTAlgorithmsService>(true);
            ClassLocator.Default.Register<IDMTAlgorithmsService>(() => ClassLocator.Default.GetInstance<DMTAlgorithmsService>(), true);
            
            // Fringe Manager
            ClassLocator.Default.Register<FringeManager>(true);
            ClassLocator.Default.Register<IFringeManager>(() => ClassLocator.Default.GetInstance<FringeManager>(), true);

            // Recipe Service
            ClassLocator.Default.Register<RecipeExecution>(true);
            ClassLocator.Default.Register<DMTRecipeService>(true);
            ClassLocator.Default.Register<IDMTRecipeService>(() => ClassLocator.Default.GetInstance<DMTRecipeService>(), true);
            
            // Screen
            ClassLocator.Default.Register<DMTScreenService>(true);
            ClassLocator.Default.Register<IDMTScreenService>(() => ClassLocator.Default.GetInstance<DMTScreenService>(), true);

            // Cameras
            // Camera Manager
            ClassLocator.Default.Register(() =>
            {
                SimulatedCameraManager = new Mock<DMTCameraManager>(
                    ClassLocator.Default.GetInstance<DMTHardwareManager>(),
                    ClassLocator.Default.GetInstance<AlgorithmManager>(),
                    ClassLocator.Default.GetInstance<CalibrationManager>(),
                    ClassLocator.Default.GetInstance<IDMTServiceConfigurationManager>(),
                    ClassLocator.Default.GetInstance<FringeManager>(),
                    ClassLocator.Default.GetInstance<ILogger<DMTCameraManager>>());
                return SimulatedCameraManager.Object;
            }, true);
            ClassLocator.Default.Register<IDMTInternalCameraMethods>(() => ClassLocator.Default.GetInstance<DMTCameraManager>(), true);
            // Camera service
            ClassLocator.Default.Register<DMTCameraService>(true);
            ClassLocator.Default.Register<IDMTCameraService>(() => ClassLocator.Default.GetInstance<DMTCameraService>(), true);
            
            //Monitor Task timer
            ClassLocator.Default.Register<MonitorTaskTimer>(true);
            ClassLocator.Default.Register(() => new Lazy<MonitorTaskTimer>(ClassLocator.Default.GetInstance<MonitorTaskTimer>));

            // Core services
            // TODO Check list with analyse
            ClassLocator.Default.Register<PMTransferManager, PMTransferManager>(true);
            ClassLocator.Default.Register<IPMStateManager, PMTransferManager>(true);
            ClassLocator.Default.Register<ICommunicationOperationsCB, PMTransferManager>(true);
            ClassLocator.Default.Register<IMaterialOperationsCB, PMTransferManager>(true);
            ClassLocator.Default.Register<IPMHandlingStatesChangedCB, PMTransferManager>(true);

            ClassLocator.Default.Register<IAlarmOperations, AlarmOperations>(true);
            ClassLocator.Default.Register<IMaterialOperations, MaterialOperations>(true);
            ClassLocator.Default.Register<ICommunicationOperations, CommunicationOperations>(true);
            ClassLocator.Default.Register<IUTOPMOperations, UTOPMOperations>(true);

            ClassLocator.Default.Register<IUTOPMService, UTOPMService>(true);
            ClassLocator.Default.Register<IUTOPMServiceCB, UTOPMService>(true);
            ClassLocator.Default.Register<IAlarmService, UTOPMService>(true);
            ClassLocator.Default.Register<ICommonEventService, UTOPMService>(true);
            ClassLocator.Default.Register<IEquipmentConstantService, UTOPMService>(true);
            ClassLocator.Default.Register<IMaterialService, UTOPMService>(true);
            ClassLocator.Default.Register<IStatusVariableService, UTOPMService>(true);
            ClassLocator.Default.Register<IAlarmServiceCB, UTOPMService>(true);
            ClassLocator.Default.Register<ICommonEventServiceCB, UTOPMService>(true);
            ClassLocator.Default.Register<IEquipmentConstantServiceCB, UTOPMService>(true);
            ClassLocator.Default.Register<IMaterialServiceCB, UTOPMService>(true);
            ClassLocator.Default.Register<IStatusVariableServiceCB, UTOPMService>(true);

            ClassLocator.Default.Register<DFSupervisor>(true);
        }
    }
}
