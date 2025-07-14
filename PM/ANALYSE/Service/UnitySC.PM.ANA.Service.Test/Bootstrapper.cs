using CommunityToolkit.Mvvm.Messaging;

using Moq;

using SimpleInjector;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Compatibility;
using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Core.Referentials;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.ANA.TC;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Implementation.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Dataflow.Proxy;
using UnitySC.Shared.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.TC.PM.Operations.Implementation;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Implementation;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Implementation;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Test
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
                    var currentConfiguration = new FakeConfigurationManager();

                    ClassLocator.ExternalInit(container, true);

                    // Init logger
                    SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);
                    ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => currentConfiguration, true);

                    // Logger with caller name
                    ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

                    // Logger without caller name
                    ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
                    ClassLocator.Default.Register<IHardwareLoggerFactory>(() => new Mock<IHardwareLoggerFactory>().Object);


                    // PM Configuration
                    ClassLocator.Default.Register<PMConfiguration>(() => PMConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().PMConfigurationFilePath), true);

                    // Flow Configuration
                    ClassLocator.Default.Register<IFlowsConfiguration>(() => FlowsConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>()), true);

                    // Calibration manager
                    ClassLocator.Default.Register<CalibrationManager>(() => new CalibrationManager(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath, false), true);

                    // Message
                    ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);

                    // Hardware manager
                    ClassLocator.Default.Register(typeof(AnaHardwareManager), typeof(AnaHardwareManager), singleton: true);
                    ClassLocator.Default.Register(typeof(HardwareManager), typeof(AnaHardwareManager), singleton: true);

                    // global status service
                    ClassLocator.Default.Register<GlobalStatusService>(true);
                    ClassLocator.Default.Register<IGlobalStatusServer>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());
                    ClassLocator.Default.Register<IGlobalStatusService>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());

                    // Probes service
                    ClassLocator.Default.Register(typeof(IProbeService), typeof(ProbeService), true);

                    // Probes service CallBack Proxy
                    ClassLocator.Default.Register(typeof(IProbeServiceCallbackProxy), typeof(ProbeService), true);

                    // Axes service
                    ClassLocator.Default.Register(typeof(IAxesService), typeof(AxesService), true);

                    // Axes service CallBack Proxy
                    ClassLocator.Default.Register(typeof(IAxesServiceCallbackProxy), typeof(AxesService), true);

                    ClassLocator.Default.Register<IReferentialManager, AnaReferentialManager>(true);

                    // Camera service
                    ClassLocator.Default.Register<CameraServiceEx>(true);
                    ClassLocator.Default.Register<ICameraServiceEx>(() => ClassLocator.Default.GetInstance<CameraServiceEx>());
                    ClassLocator.Default.Register<ICameraManager>(() => ClassLocator.Default.GetInstance<USPCameraManager>());

                    // PM User service
                    ClassLocator.Default.Register(typeof(IPMUserService), typeof(PMUserService), true);

                    // Calibration Service
                    ClassLocator.Default.Register(typeof(ICalibrationService), typeof(CalibrationService), true);

                    // Calibration Service
                    ClassLocator.Default.Register(typeof(IAlgoService), typeof(AlgoService), true);

                    // Chamber service
                    ClassLocator.Default.Register(typeof(IChamberService), typeof(ChamberService), true);

                    // Ana Recipe execution manager
                    ClassLocator.Default.Register(typeof(IANARecipeExecutionManager), typeof(ANARecipeExecutionManagerDummy), true);

                    // Ana recipe service
                    ClassLocator.Default.Register(typeof(IANARecipeService), typeof(ANARecipeService), true);

                    // Compatibility manager
                    ClassLocator.Default.Register<CompatibilityManager>(true);

                    // Proxy for Recipe service in DataAccess
                    ClassLocator.Default.Register<DbRecipeServiceProxy>(true);

                    // Proxy for Tool service in DataAcces
                    ClassLocator.Default.Register<DbToolServiceProxy>(true);

                    // Measure configuration
                    ClassLocator.Default.Register<MeasuresConfiguration>(() => MeasuresConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>()), true);

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
                }
            }
        }
    }
}
