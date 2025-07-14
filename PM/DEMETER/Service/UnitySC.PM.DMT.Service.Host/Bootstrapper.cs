using System;

using CommunityToolkit.Mvvm.Messaging;

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
using UnitySC.PM.DMT.Service.Interface.Chamber;
using UnitySC.PM.DMT.Service.Interface.Chuck;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.DMT.Service.Interface.Proxy;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Hardware.Service.Interface.Global;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.FDC.Service;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.MonitorTasks;

namespace UnitySC.PM.DMT.Service.Host
{
    public static class Bootstrapper
    {
        private const string PMConfigurationFilePath = @"PMConfiguration.xml";

        public static void Register(string[] args = null)
        {
            // Configuration
            var currentConfiguration = new DMTServiceConfigurationManager(args);

            //Automation Configuration
            var currentAutomationConfiguration = new AutomationConfiguration(ActorType.DEMETER, currentConfiguration?.ConfigurationName);
            ClassLocator.Default.Register<IAutomationConfiguration>(() => currentAutomationConfiguration, true);

            // Init logger
            SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);
            ClassLocator.Default.Register<IDMTServiceConfigurationManager>(() => currentConfiguration, true);
            ClassLocator.Default.Register<IServiceConfigurationManager>(() => currentConfiguration, true);
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => currentConfiguration, true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register<ILogger, SerilogLogger<object>>();

            //Register logger factory for hardware
            ClassLocator.Default.Register<Func<string, string, string, IHardwareLogger>>(() =>
                (logLevel, hardwareFamily, deviceName) => new HardwareLogger(logLevel, hardwareFamily, deviceName));

            // Register  LoggerFactory
            ClassLocator.Default.Register<IHardwareLoggerFactory, HardwareLoggerFactory>();

            // Recipe execution manager
            ClassLocator.Default.Register<RecipeExecution>(true);

            // Recipe service
            ClassLocator.Default.Register<DMTRecipeService>(true);
            ClassLocator.Default.Register<IDMTRecipeService>(() => ClassLocator.Default.GetInstance<DMTRecipeService>(), true);
            ClassLocator.Default.Register<DMTAlgorithmsService>(true);
            ClassLocator.Default.Register<IDMTAlgorithmsService>(() => ClassLocator.Default.GetInstance<DMTAlgorithmsService>(), true);

            // Calibration service
            ClassLocator.Default.Register<ICalibrationService, DMTCalibrationService>(true);
            ClassLocator.Default.Register<CalibrationManager>(true);
            ClassLocator.Default.Register<ICalibrationManager>(() => ClassLocator.Default.GetInstance<CalibrationManager>(), true);


            // Hardware service
            ClassLocator.Default.Register<DMTCameraService>(true);
            ClassLocator.Default.Register<IDMTCameraService>(() => ClassLocator.Default.GetInstance<DMTCameraService>(), true);
            ClassLocator.Default.Register<DMTCameraManager>(true);
            ClassLocator.Default.Register<ICameraManager>(() => ClassLocator.Default.GetInstance<DMTCameraManager>(), true);
            ClassLocator.Default.Register<IDMTInternalCameraMethods>(() => ClassLocator.Default.GetInstance<DMTCameraManager>(), true);
            ClassLocator.Default.Register<IDMTScreenService, DMTScreenService>(true);
            ClassLocator.Default.Register<IChamberService, ChamberService>(true);
            ClassLocator.Default.Register<IDMTChamberService, DMTChamberService>(true);
            ClassLocator.Default.Register<IChuckService, ChuckService>(true);
            ClassLocator.Default.Register<IDMTChuckService, DMTChuckService>(true);
            ClassLocator.Default.Register<IMotionAxesServiceCallbackProxy, MotionAxesService>(true);
            ClassLocator.Default.Register<IFfuService, FfuService>(true);

            // Axes service
            ClassLocator.Default.Register<IAxesService, AxesService>(true);
            // Axes service CallBack Proxy
            ClassLocator.Default.Register<IAxesServiceCallbackProxy, AxesService>(true);

            // Chuck service CallBack Proxy
            ClassLocator.Default.Register<IChuckServiceCallbackProxy, ChuckService>(true);

            ClassLocator.Default.Register<IReferentialManager, EmptyReferentialManager>(true);

            // Plc service
            ClassLocator.Default.Register<IPlcService, PlcService>(true);

            // Global device service
            ClassLocator.Default.Register<IGlobalDeviceService, GlobalDeviceService>(true);

            // Hardware manager
            ClassLocator.Default.Register<DMTHardwareManager>(true);
            ClassLocator.Default.Register<HardwareManager>(() => ClassLocator.Default.GetInstance<DMTHardwareManager>(), true);
            ClassLocator.Default.Register<IHardwareManager>(() => ClassLocator.Default.GetInstance<DMTHardwareManager>(), true);

            // Motion axes service
            ClassLocator.Default.Register<IMotionAxesService, MotionAxesService>(true);

            // Fringe manager
            ClassLocator.Default.Register<FringeManager>(true);
            ClassLocator.Default.Register<IFringeManager>(() => ClassLocator.Default.GetInstance<FringeManager>(), true);

            // Algorithm manager
            ClassLocator.Default.Register<AlgorithmManager>(true);
            ClassLocator.Default.Register<IDMTAlgorithmManager>(() => ClassLocator.Default.GetInstance<AlgorithmManager>(), true);

            // PM Configuration
            ClassLocator.Default.Register(() => PMConfiguration.Init(currentConfiguration.PMConfigurationFilePath),
                true);
            ClassLocator.Default.Register<ModuleConfiguration>(() => ClassLocator.Default.GetInstance<PMConfiguration>(), true);

            // Flows
            ClassLocator.Default.Register(() => FlowsConfiguration.Init(currentConfiguration), true);
            ClassLocator.Default.Register<IFlowsConfiguration>(() => ClassLocator.Default.GetInstance<FlowsConfiguration>(), true);
            ClassLocator.Default.Register(typeof(ContextApplier<>), typeof(DMTContextApplier<>), true);

            // Measures configuration
            ClassLocator.Default.Register(() => MeasuresConfiguration.Init(currentConfiguration), true);

            // Dataflow
            ClassLocator.Default.Register<DapProxy>(true);
            ClassLocator.Default.Register<DbRecipeServiceProxy>(true);
            ClassLocator.Default.Register<DbToolServiceProxy>(true);
            ClassLocator.Default.Register<DbRegisterResultServiceProxy>(true);

            // Message
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);

            ClassLocator.Default.Register<GlobalStatusService>(true);
            ClassLocator.Default.Register<IGlobalStatusServer>(() =>
                ClassLocator.Default.GetInstance<GlobalStatusService>());
            ClassLocator.Default.Register<IGlobalStatusService>(() =>
                ClassLocator.Default.GetInstance<GlobalStatusService>());

            ClassLocator.Default.Register<IPMUserService, PMUserService>(true);

            // FDCs
            ClassLocator.Default.Register(() => new FDCManager(currentConfiguration.FDCsConfigurationFilePath, currentConfiguration.FDCsPersistentDataFilePath));
            ClassLocator.Default.Register<IFDCService, FDCService>(true);

            //UTO Objects registering
            TC.Bootstrapper.Register();
            
            //Monitor Task Timer
            ClassLocator.Default.Register<MonitorTaskTimer>(true);
            ClassLocator.Default.Register(() => new Lazy<MonitorTaskTimer>(ClassLocator.Default.GetInstance<MonitorTaskTimer>), true);
        }
    }
}
