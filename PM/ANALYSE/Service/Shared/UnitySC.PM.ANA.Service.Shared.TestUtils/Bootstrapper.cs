using CommunityToolkit.Mvvm.Messaging;

using Moq;

using Serilog.Events;

using SimpleInjector;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Context;
using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.ANA.TC;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Dataflow.Proxy;
using UnitySC.Shared.FDC;
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

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public static class Bootstrapper
    {
        public static Mock<ICameraManager> SimulatedCameraManager { get; private set; }

        public static Mock<IReferentialManager> SimulatedReferentialManager { get; private set; }

        // Event forwarding from C++ algorithm library to Host application
        private static UnitySCSharedAlgosOpenCVWrapper.ManagedEventQueue s_algorithmLoggerService;

        private static EventForwarder s_eventForwarder;

        public static void Register(Container container, bool flowsAreSimulated = false)
        {
            // Configuration
            var configurationManager = new FakeConfigurationManager(flowsAreSimulated);

            // Init logger
            SerilogInit.Init(configurationManager.LogConfigurationFilePath);
            ClassLocator.ExternalInit(container, true);

            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => configurationManager, true);
            ClassLocator.Default.Register(() => MeasuresConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>()), true);

            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            //ClassLocator.Default.Register<Func<string, string, string, IHardwareLogger>>(() =>
            //  (logLevel, hardwareFamily, deviceName) => new HardwareLogger(logLevel, hardwareFamily, deviceName));


            var mockLogger = Mock.Of<IHardwareLogger>();
            var mockLoggerFactory = Mock.Of<IHardwareLoggerFactory>(x => x.CreateHardwareLogger(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) == mockLogger);
            ClassLocator.Default.Register<IHardwareLoggerFactory>(() =>mockLoggerFactory);

            // PM Configuration
            ClassLocator.Default.Register<PMConfiguration>(() => PMConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().PMConfigurationFilePath), true);

            // Flow Configuration
            ClassLocator.Default.Register<IFlowsConfiguration>(() => FlowsConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>()), true);

            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);
            ClassLocator.Default.Register<GlobalStatusService>(true);
            ClassLocator.Default.Register<IGlobalStatusServer>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());

            ClassLocator.Default.Register<AnaHardwareManager>(true);
            ClassLocator.Default.Register<HardwareManager, AnaHardwareManager>(true);

            // Probes
            ClassLocator.Default.Register<IProbeService, ProbeService>(true);
            ClassLocator.Default.Register<IProbeServiceCallbackProxy, ProbeService>(true);

            // Axes
            ClassLocator.Default.Register<IAxesService, AxesService>(true);
            ClassLocator.Default.Register<IAxesServiceCallbackProxy, AxesService>(true);

            // Camera
            //ClassLocator.Default.Register<ICameraManager, USPCameraManager>(true);
            SimulatedCameraManager = new Mock<ICameraManager>();
            ClassLocator.Default.Register<ICameraManager>(() => SimulatedCameraManager.Object);

            // Referential manager
            SimulatedReferentialManager = new Mock<IReferentialManager>();
            ClassLocator.Default.Register(() => SimulatedReferentialManager.Object);

            // Calibration
            ClassLocator.Default.Register<CalibrationManager>(() => new CalibrationManager(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath, false), true);
            ClassLocator.Default.Register(typeof(ICalibrationService), typeof(CalibrationService), true);

            // PM User service
            ClassLocator.Default.Register(typeof(IPMUserService), typeof(PMUserService), true);

            // Flow initial context applier
            ClassLocator.Default.Register(typeof(IContextManager), typeof(ContextManager), true);
            ClassLocator.Default.Register(typeof(ContextApplier<>), typeof(FlowInitialContextApplier));

            // Algo Service
            ClassLocator.Default.Register(typeof(IAlgoService), typeof(AlgoService), true);

            // Services
            ClassLocator.Default.Register<PMTransferManager, PMTransferManager>(singleton: true);
            ClassLocator.Default.Register<IPMStateManager, PMTransferManager>(singleton: true);
            ClassLocator.Default.Register<IAlarmOperationsCB, AnaPMTCManager>(singleton: true);
            ClassLocator.Default.Register<ICommunicationOperationsCB, PMTransferManager>(singleton: true);
            ClassLocator.Default.Register<IMaterialOperationsCB, PMTransferManager>(singleton: true);
            ClassLocator.Default.Register<IPMHandlingStatesChangedCB, PMTransferManager>(singleton: true);

            ClassLocator.Default.Register<IAlarmOperations, AlarmOperations>(singleton: true);
            ClassLocator.Default.Register<IMaterialOperations, MaterialOperations>(singleton: true);
            ClassLocator.Default.Register<ICommunicationOperations, CommunicationOperations>(singleton: true);
            ClassLocator.Default.Register<IUTOPMOperations, UTOPMOperations>(singleton: true);

            ClassLocator.Default.Register<IUTOPMService, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<IUTOPMServiceCB, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<IAlarmService, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<ICommonEventService, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<IEquipmentConstantService, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<IMaterialService, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<IStatusVariableService, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<IAlarmServiceCB, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<ICommonEventServiceCB, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<IEquipmentConstantServiceCB, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<IMaterialServiceCB, UTOPMService>(singleton: true);
            ClassLocator.Default.Register<IStatusVariableServiceCB, UTOPMService>(singleton: true);

            ClassLocator.Default.Register(typeof(IPMTCManager), typeof(AnaPMTCManager), singleton: true);
            ClassLocator.Default.Register(typeof(IANAHandling), typeof(AnaHandlingManager), singleton: true);
            ClassLocator.Default.Register<DFSupervisor>(true);

            //FDCs
            ClassLocator.Default.Register(() => new FDCManager("test", "test"), true);
            ClassLocator.Default.Register(() => new BareWaferAlignmentFlowFDCProvider(), true);
            ClassLocator.Default.Register(() => new ANARecipeExecutionManagerFDCProvider(), true);
            ClassLocator.Default.Register(() => new CalibrationManagerFDCProvider(), true);

            // Forward logging from algo library
            ILogger logger = new SerilogLogger<object>();
            s_algorithmLoggerService = new UnitySCSharedAlgosOpenCVWrapper.ManagedEventQueue();
            s_eventForwarder = new EventForwarder(logger, "[ALGOS]");
            s_algorithmLoggerService.AddMessageEventHandler(s_eventForwarder.ForwardEvent);
        }
    }
}
