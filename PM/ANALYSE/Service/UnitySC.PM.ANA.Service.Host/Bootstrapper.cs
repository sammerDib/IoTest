using System;
using System.IO;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Proxy;
using UnitySC.PM.ANA.EP.Mountains.Server.Implementation;
using UnitySC.PM.ANA.EP.Shared;
using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.AutofocusV2;
using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Compatibility;
using UnitySC.PM.ANA.Service.Core.Context;
using UnitySC.PM.ANA.Service.Core.MeasureCalibration;
using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Core.Referentials;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Alignment;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.PM.ANA.Service.Interface.Compatibility;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Measure.AutofocusTracker;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Loader;
using UnitySC.PM.ANA.Service.Shared.Interface;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Implementation.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
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
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Host
{
    public static class Bootstrapper
    {
        private const string ServiceConfigurationWlFileName = "ServiceConfigurationWaferLess.xml";

        public static void Register(string[] args = null)
        {
            // Configuration
            var currentConfiguration = new PMServiceConfigurationManager(args);
            var externalProcessingConfiguration = ExternalProcessingConfiguration.Init(currentConfiguration.ConfigurationFolderPath);

            //Automation Configuration
            var currentAutomationConfiguration = new AutomationConfiguration(ActorType.ANALYSE, currentConfiguration?.ConfigurationName);

            // Init logger
            SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);

            // Configurations
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => currentConfiguration, true);
            ClassLocator.Default.Register<ExternalProcessingConfiguration>(() => externalProcessingConfiguration, true);
            ClassLocator.Default.Register<IAutomationConfiguration>(() => currentAutomationConfiguration, true);
            ClassLocator.Default.Register<ServiceConfigurationWaferLess>(() => ServiceConfigurationWaferLess.Init(Path.Combine(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().ConfigurationFolderPath, ServiceConfigurationWlFileName)), true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));


            //Register logger factory for hardware
            ClassLocator.Default.Register<Func<string, string, string, IHardwareLogger>>(() =>
                (logLevel, hardwareFamily, deviceName) => new HardwareLogger(logLevel, hardwareFamily, deviceName));

            // Register  LoggerFactory
            ClassLocator.Default.Register<IHardwareLoggerFactory, HardwareLoggerFactory>();


            // Message
            ClassLocator.Default.Register<CommunityToolkit.Mvvm.Messaging.IMessenger, CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger>(true);

            ClassLocator.Default.Register<ServiceInvoker<IDbRecipeService>>(() => new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress()));
            // Hardware manager
            ClassLocator.Default.Register(typeof(AnaHardwareManager), typeof(AnaHardwareManager), singleton: true);
            ClassLocator.Default.Register(typeof(HardwareManager), typeof(AnaHardwareManager), singleton: true);
            ClassLocator.Default.Register<IHardwareManager>(() => ClassLocator.Default.GetInstance<AnaHardwareManager>());

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
            ClassLocator.Default.Register(typeof(IMotionAxesServiceCallbackProxy), typeof(MotionAxesService), true);

            // Chuck service
            ClassLocator.Default.Register(typeof(IChuckService), typeof(ChuckService), true);

            // Chuck service CallBack Proxy
            ClassLocator.Default.Register(typeof(IChuckServiceCallbackProxy), typeof(ChuckService), true);

            // Camera service
            ClassLocator.Default.Register<CameraServiceEx>(true);
            ClassLocator.Default.Register<ICameraServiceEx>(() => ClassLocator.Default.GetInstance<CameraServiceEx>());
            ClassLocator.Default.Register<ICameraManager, USPCameraManager>(true);

            // Light service
            ClassLocator.Default.Register(typeof(ILightService), typeof(LightService), true);

            // Light service CallBack Proxy
            ClassLocator.Default.Register(typeof(ILightServiceCallbackProxy), typeof(LightService), true);            

            // PM User service
            ClassLocator.Default.Register(typeof(IPMUserService), typeof(PMUserService), true);

            // Calibration Service
            ClassLocator.Default.Register(typeof(ICalibrationService), typeof(CalibrationService), true);

            // Alignment Service
            ClassLocator.Default.Register(typeof(IProbeAlignmentService), typeof(ProbeAlignmentService), true);

            // Compatibility Service
            ClassLocator.Default.Register(typeof(ICompatibilityService), typeof(CompatibilityService), true);

            // Algorithm Service
            ClassLocator.Default.Register(typeof(IAlgoService), typeof(AlgoService), true);

            // Chamber service
            ClassLocator.Default.Register(typeof(IChamberService), typeof(ChamberService), true);

            // FDC service
            ClassLocator.Default.Register(typeof(IFDCService), typeof(FDCService), true);

            // Alignment Service
            ClassLocator.Default.Register(typeof(IClientFDCsService), typeof(ClientFDCsService), true);


            // Controller service
            ClassLocator.Default.Register(typeof(IControllerService), typeof(ControllerService), true);
            if (externalProcessingConfiguration.Mountains != null)
            {
                if (externalProcessingConfiguration.Mountains.IsHostedByPM)
                {
                    ClassLocator.Default.Register<MountainsConfiguration>(() => externalProcessingConfiguration.Mountains, true);
                    ClassLocator.Default.Register<MountainsActiveXSupervisor>(true);
                }
                ClassLocator.Default.Register(typeof(IMountainsGatewayService), typeof(MountainsGatewayService), true);
                ClassLocator.Default.Register<MountainsSupervisor>(() => new MountainsSupervisor(externalProcessingConfiguration.Mountains.Address), true);
            }

            // Ana recipe service
            ClassLocator.Default.Register(typeof(IANARecipeService), typeof(ANARecipeService), true);

            // Recipe execution manager
            if (currentConfiguration.FlowsAreSimulated)
                ClassLocator.Default.Register(typeof(IANARecipeExecutionManager), typeof(ANARecipeExecutionManagerDummy), true);
            else
                ClassLocator.Default.Register(typeof(IANARecipeExecutionManager), typeof(ANARecipeExecutionManager), true);

            // PM Configuration
            ClassLocator.Default.Register<PMConfiguration>(() => PMConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().PMConfigurationFilePath), true);
            ClassLocator.Default.Register<ModuleConfiguration>(() => ClassLocator.Default.GetInstance<PMConfiguration>(), true);

            // Flow Configuration
            ClassLocator.Default.Register<IFlowsConfiguration>(() => FlowsConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>()), true);

            // Calibration manager
            ClassLocator.Default.Register<CalibrationManager>(() => new CalibrationManager(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath), true);

            // Proxy for Recipe service in DataAccess
            ClassLocator.Default.Register<DbRecipeServiceProxy>(true);

            // Proxy for Tool service in DataAccess
            ClassLocator.Default.Register<DbToolServiceProxy>(true);

            // Referential management
            ClassLocator.Default.Register(typeof(IReferentialManager), typeof(AnaReferentialManager), true);

            // Referential service
            ClassLocator.Default.Register(typeof(IReferentialService), typeof(ReferentialService), true);

            // Compatibility manager
            ClassLocator.Default.Register<CompatibilityManager>(true);

            // Measure configuration
            ClassLocator.Default.Register<MeasuresConfiguration>(() => MeasuresConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>()), true);

            // Measure Loader
            ClassLocator.Default.Register<MeasureLoader>(true);

            // Measure AutofocusTracker
            ClassLocator.Default.Register<MeasureAutofocusTracker>(true);

            // Measure service
            ClassLocator.Default.Register(typeof(IMeasureService), typeof(MeasureService), true);

            // Context
            ClassLocator.Default.Register(typeof(IContextService), typeof(ContextService), true);
            ClassLocator.Default.Register(typeof(IContextManager), typeof(ContextManager), true);
            ClassLocator.Default.Register(typeof(ContextApplier<>), typeof(FlowInitialContextApplier));

            // Laser service
            ClassLocator.Default.Register(typeof(ILaserService), typeof(LaserService), true);

            // Shutter service
            ClassLocator.Default.Register(typeof(IShutterService), typeof(ShutterService), true);

            // Plc service
            ClassLocator.Default.Register(typeof(IPlcService), typeof(PlcService), true);

            // FFU service
            ClassLocator.Default.Register(typeof(IFfuService), typeof(FfuService), true);

            // Motion axes service
            ClassLocator.Default.Register(typeof(IMotionAxesService), typeof(MotionAxesService), true);

            //PMTCService - API interface TC for PM
            UnitySC.PM.ANA.TC.Bootstrapper.Register();

            // Spectro service
            ClassLocator.Default.Register(typeof(ISpectroService), typeof(SpectroService), true);
            // Spectro service CallBack Proxy
            ClassLocator.Default.Register(typeof(ISpectroServiceCallbackProxy), typeof(SpectroService), true);

            ClassLocator.Default.Register(typeof(UnitySC.Shared.Tools.MonitorTasks.MonitorTaskTimer), typeof(UnitySC.Shared.Tools.MonitorTasks.MonitorTaskTimer), true);

            //FDCs
            ClassLocator.Default.Register(() => new FDCManager(currentConfiguration.FDCsConfigurationFilePath, currentConfiguration.FDCsPersistentDataFilePath), true);
            ClassLocator.Default.Register(() => new ChamberFDCs(), true);
            ClassLocator.Default.Register(() => new ClientFDCs(), true);
            ClassLocator.Default.Register(() => new BareWaferAlignmentFlowFDCProvider(), true);
            ClassLocator.Default.Register(() => new PatternRecFlowFDCProvider(), true);
            ClassLocator.Default.Register(() => new AFLiseFlowFDCProvider(), true);
            ClassLocator.Default.Register(() => new AFCameraFlowFDCProvider(), true);
            ClassLocator.Default.Register(() => new ANARecipeExecutionManagerFDCProvider(), true);
            ClassLocator.Default.Register(() => new CalibrationManagerFDCProvider(), true);
            ClassLocator.Default.Register(() => new ProbeLiseHFFDCProvider(), true);

            ClassLocator.Default.Register(typeof(IProbeCalibrationManagerInit), typeof(ProbeCalibrationManagerInit), true);
            ClassLocator.Default.Register<AnaServer>(true);
        }
    }
}
