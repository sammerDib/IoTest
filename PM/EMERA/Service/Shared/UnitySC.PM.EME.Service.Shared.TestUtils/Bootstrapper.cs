using CommunityToolkit.Mvvm.Messaging;

using Moq;

using SimpleInjector;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Calibration;
using UnitySC.PM.EME.Service.Core.Context;
using UnitySC.PM.EME.Service.Core.Referentials;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Implementation;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Shared.TestUtils
{
    public static class Bootstrapper
    {
        // Event forwarding from C++ algorithm library to Host application
        private static UnitySCSharedAlgosOpenCVWrapper.ManagedEventQueue s_algorithmLoggerService;

        private static EventForwarder s_eventForwarder;

        public static Mock<IEmeraCamera> SimulatedEmeraCamera { get; private set; }

        public static void Register(Container container, bool flowsAreSimulated = false)
        {
            // Configuration
            var configurationManager = new FakeConfigurationManager(flowsAreSimulated);

            // Init logger
            SerilogInit.Init(configurationManager.LogConfigurationFilePath);
            ClassLocator.ExternalInit(container, true);

            ClassLocator.Default.Register<IEMEServiceConfigurationManager>(() => configurationManager, true);
            ClassLocator.Default.Register<IServiceConfigurationManager>(() => configurationManager, true);
            ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => configurationManager, true);

            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            var mockLogger = Mock.Of<IHardwareLogger>();
            var mockLoggerFactory = Mock.Of<IHardwareLoggerFactory>(x => x.CreateHardwareLogger(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()) == mockLogger);
            ClassLocator.Default.Register<IHardwareLoggerFactory>(() => mockLoggerFactory);

            // PM Configuration
            ClassLocator.Default.Register<PMConfiguration>(() => EmePMConfiguration.Init(configurationManager.PMConfigurationFilePath), true);

            // Flow Configuration
            ClassLocator.Default.Register<IFlowsConfiguration>(() => FlowsConfiguration.Init(configurationManager), true);
            //Recipe Configuration
            ClassLocator.Default.Register(() => RecipeConfiguration.Init(configurationManager), true);

            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
            ClassLocator.Default.Register<GlobalStatusService>(true);
            ClassLocator.Default.Register<IGlobalStatusServer>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());

            ClassLocator.Default.Register<EmeHardwareManager>(true);
            ClassLocator.Default.Register<HardwareManager, EmeHardwareManager>(true);

            // Motion Axes
            ClassLocator.Default.Register(typeof(IMotionAxesServiceCallbackProxy), typeof(EmeraMotionAxesService), true);
            ClassLocator.Default.Register(typeof(IEmeraMotionAxesService), typeof(EmeraMotionAxesService), true);

            ClassLocator.Default.Register(typeof(IReferentialManager), typeof(EmeReferentialManager), true);

            // Camera
            SimulatedEmeraCamera = new Mock<IEmeraCamera>();
            ClassLocator.Default.Register<IEmeraCamera>(() => SimulatedEmeraCamera.Object);


            //Calibration            
            ClassLocator.Default.Register(() => new CalibrationManager(configurationManager.CalibrationFolderPath, true), true);
            ClassLocator.Default.Register<ICalibrationManager>(() => new CalibrationManager(configurationManager.CalibrationFolderPath, true), true);
            ClassLocator.Default.Register<ICalibrationService, CalibrationService>(true);

            //Referential 
            ClassLocator.Default.Register(typeof(IReferentialService), typeof(ReferentialService), true);

            // Flow initial context applier
            ClassLocator.Default.Register(typeof(IContextManager), typeof(ContextManager), true);
            ClassLocator.Default.Register(typeof(ContextApplier<>), typeof(FlowInitialContextApplier));

            //TODO : Add AlgoService

            // Forward logging from algo library
            ILogger logger = new SerilogLogger<object>();
            s_algorithmLoggerService = new UnitySCSharedAlgosOpenCVWrapper.ManagedEventQueue();
            s_eventForwarder = new EventForwarder(logger, "[ALGOS]");
            s_algorithmLoggerService.AddMessageEventHandler(s_eventForwarder.ForwardEvent);
        }
    }
}
