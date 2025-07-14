using System;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Context;
using UnitySC.PM.EME.Service.Core.Recipe.Save;
using UnitySC.PM.EME.Service.Core.Referentials;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Core.Shared.DateTimeHelper;
using UnitySC.PM.EME.Service.Core.Shared.DateTimeProvider;
using UnitySC.PM.EME.Service.Implementation;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Camera;
using UnitySC.PM.EME.Service.Interface.Chamber;
using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.PM.EME.Service.Interface.Chuck;
using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Implementation.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Proxy;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.FDC.Service;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Host
{
    public static class Bootstrapper
    {
        public static void Register(string[] args = null)
        {
            var currentConfiguration = new EMEServiceConfigurationManager(args);
            //Automation Configuration
            var currentAutomationConfiguration = new AutomationConfiguration(ActorType.EMERA, currentConfiguration?.ConfigurationName);
            ClassLocator.Default.Register<IAutomationConfiguration>(() => currentAutomationConfiguration, true);

            ClassLocator.Default.Register<IEMEServiceConfigurationManager>(() => currentConfiguration, true);
            ClassLocator.Default.Register<IServiceConfigurationManager>(() => currentConfiguration, true);
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => currentConfiguration, true);
            ClassLocator.Default.Register(() => EmePMConfiguration.Init(currentConfiguration.PMConfigurationFilePath), true);            
            ClassLocator.Default.Register<ModuleConfiguration>(() => ClassLocator.Default.GetInstance<PMConfiguration>(), true);            
          
            RegisterMessenger();
            RegisterLogger(currentConfiguration.LogConfigurationFilePath);
            RegisterGlobalStatusService();
            RegisterHardwareManager();
            RegisterCameraService();
            RegisterCalibrationService();
            RegisterCalibrationManager();
            RegisterChamberService();
            RegisterChuckService();
            RegisterReferentialManager();
            RegisterMotionAxes();
            RegisterFilterWheel();
            RegisterContext();
            RegisterFlowsConfiguration();
            RegisterRecipeConfiguration();
            RegisterAlgoService();
            RegisterReferentialService();
            RegisterDBServiceProxy();
            RegisterEMERecipeService();
            RegisterImageFileService();
            RegisterLightService();
            RegisterAdaFileService();
            RegisterRecipeAcquisitionTemplateComposer();
            RegisterDateTimeHelper();
            RegisterDistanceSensorService();
            RegisterPlcService();
            RegisterDFSupervisor();
            RegisterUTOService();
            RegisterChillerService();

            ClassLocator.Default.Register(typeof(IPMUserService), typeof(PMUserService), true);

            ClassLocator.Default.Register(() => new FDCManager(currentConfiguration.FDCsConfigurationFilePath, currentConfiguration.FDCsPersistentDataFilePath));
            ClassLocator.Default.Register<IFDCService, FDCService>(true);

        }

        private static void RegisterDFSupervisor()
        {            
            ClassLocator.Default.Register(typeof(IPMDFService), typeof(DFSupervisor), singleton: true);
            ClassLocator.Default.Register(typeof(IPMDFServiceCB), typeof(DFSupervisor), singleton: true);            
        }

        private static void RegisterUTOService()
        {
            //PMTCService - API interface TC for PM
            UnitySC.PM.EME.TC.Bootstrapper.Register();

            
        }

        private static void RegisterChillerService()
        {
            ClassLocator.Default.Register(typeof(IChillerService), typeof(ChillerService), true);
        }

        private static void RegisterPlcService()
        {
            ClassLocator.Default.Register(typeof(IPlcService), typeof(PlcService), true);
        }

        private static void RegisterChuckService()
        {
            ClassLocator.Default.Register(typeof(IChuckService), typeof(ChuckService), true);
            ClassLocator.Default.Register<IEMEChuckService, EMEChuckService>(true);
            ClassLocator.Default.Register(typeof(IChuckServiceCallbackProxy), typeof(EMEChuckService), true);
        }

        private static void RegisterAdaFileService()
            => ClassLocator.Default.Register(typeof(IAdaFileSaver), typeof(AdaFileSaver), true);

        private static void RegisterImageFileService()
            => ClassLocator.Default.Register(typeof(IImageFileSaver), typeof(ImageFileSaver), true);

        private static void RegisterRecipeAcquisitionTemplateComposer()
            => ClassLocator.Default.Register(typeof(IRecipeAcquisitionTemplateComposer), typeof(RecipeAcquisitionTemplateComposer), true);

        private static void RegisterEMERecipeService()
        {
           // ClassLocator.Default.Register(typeof(IEMERecipeExecution), typeof(RecipeExecution), true);
            ClassLocator.Default.Register(typeof(IEMERecipeService), typeof(EMERecipeService), true);            
        }

        private static void RegisterDBServiceProxy()
        {
            // Proxy for Recipe service in DataAccess
            ClassLocator.Default.Register<DbRecipeServiceProxy>(true);
        }

        private static void RegisterReferentialService()
        {
            ClassLocator.Default.Register(typeof(IReferentialService), typeof(ReferentialService), true);
        }

        private static void RegisterAlgoService()
        {
            ClassLocator.Default.Register(typeof(IAlgoService), typeof(AlgoService), true);
        }

        private static void RegisterFlowsConfiguration()
        {
            ClassLocator.Default.Register<IFlowsConfiguration>(() => FlowsConfiguration.Init(ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>()), true);
        }

        private static void RegisterContext()
        {
            ClassLocator.Default.Register(typeof(IContextManager), typeof(ContextManager), true);
            ClassLocator.Default.Register(typeof(ContextApplier<>), typeof(FlowInitialContextApplier));
        }

        private static void RegisterChamberService()
        {
            ClassLocator.Default.Register(typeof(IEMEChamberService), typeof(EMEChamberService), singleton: true);
        }

        private static void RegisterMessenger()
        {
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
        }

        private static void RegisterCameraService()
        {
            ClassLocator.Default.Register<IEmeraCamera, EmeraCamera>(true);
            ClassLocator.Default.Register<ICameraManager, USPMilCameraManager>(true);
            ClassLocator.Default.Register<CameraServiceEx>(true);
            ClassLocator.Default.Register<ICameraServiceEx, CameraServiceEx>(true);
        }

        private static void RegisterCalibrationService()
        {
            ClassLocator.Default.Register<ICalibrationService, CalibrationService>(true);
        }

        private static void RegisterCalibrationManager()
        {
            ClassLocator.Default.Register(() => new CalibrationManager(ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>().CalibrationFolderPath), true);
            ClassLocator.Default.Register<ICalibrationManager>(() => new CalibrationManager(ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>().CalibrationFolderPath), true);
        }

        private static void RegisterHardwareManager()
        {
            ClassLocator.Default.Register(typeof(EmeHardwareManager), typeof(EmeHardwareManager), singleton: true);
            ClassLocator.Default.Register(typeof(HardwareManager), typeof(EmeHardwareManager), singleton: true);
            ClassLocator.Default.Register(typeof(IHardwareManager), typeof(EmeHardwareManager), singleton: true);
        }

        private static void RegisterGlobalStatusService()
        {
            ClassLocator.Default.Register<GlobalStatusService>(true);
            ClassLocator.Default.Register(typeof(IGlobalStatusServer), typeof(GlobalStatusService), singleton: true);
            ClassLocator.Default.Register(typeof(IGlobalStatusService), typeof(GlobalStatusService), singleton: true);
        }

        private static void RegisterLogger(string logConfiguration)
        {
            SerilogInit.Init(logConfiguration);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            //Register logger factory for hardware
            ClassLocator.Default.Register<Func<string, string, string, IHardwareLogger>>(() =>
                (logLevel, hardwareFamily, deviceName) => new HardwareLogger(logLevel, hardwareFamily, deviceName));
            ClassLocator.Default.Register<IHardwareLoggerFactory, HardwareLoggerFactory>();
        }

        private static void RegisterReferentialManager()
        {
            ClassLocator.Default.Register(typeof(IReferentialManager), typeof(EmeReferentialManager), true);
        }

        private static void RegisterMotionAxes()
        {
            ClassLocator.Default.Register(typeof(IMotionAxesServiceCallbackProxy), typeof(EmeraMotionAxesService), true);
            ClassLocator.Default.Register(typeof(IEmeraMotionAxesService), typeof(EmeraMotionAxesService), true);
        }

        private static void RegisterFilterWheel()
        {
            ClassLocator.Default.Register(typeof(IFilterWheelService), typeof(FilterWheelService), true);
        }

        private static void RegisterRecipeConfiguration()
        {
            ClassLocator.Default.Register(() => RecipeConfiguration.Init(ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>()), true);
        }

        private static void RegisterLightService()
        {
            ClassLocator.Default.Register(typeof(IEMELightService), typeof(EMELightService), true);
        }

        private static void RegisterDistanceSensorService()
        {
            ClassLocator.Default.Register(typeof(IDistanceSensorService), typeof(DistanceSensorService), true);
        }

        private static void RegisterDateTimeHelper()
            => ClassLocator.Default.Register(typeof(IDateTimeProvider), typeof(DateTimeProvider), true);
               
    }
}
