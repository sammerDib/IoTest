using System.IO;

using CommunityToolkit.Mvvm.Messaging;

using Moq;

using SimpleInjector;

using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Implementation.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Implementation;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Service.Test
{
    public partial class Bootstrapper

    {
        public static Mock<IReferentialManager> SimulatedReferentialManager { get; private set; }
        private const string PMConfigurationFilePath = @"PMConfiguration.xml";

        public static void Register(Container container)
        {
            ClassLocator.ExternalInit(container, true);

            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // global status service
            ClassLocator.Default.Register<GlobalStatusService>(true);
            ClassLocator.Default.Register<IGlobalStatusServer>(() =>
            ClassLocator.Default.GetInstance<GlobalStatusService>());

            // Message
            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);

            // Axes
            ClassLocator.Default.Register<IAxes, NSTAxes>(true);
            ClassLocator.Default.Register<IAxesService, AxesService>(true);
            ClassLocator.Default.Register<IAxesServiceCallbackProxy, AxesService>(true);

            // Camera service
            ClassLocator.Default.Register<ICameraManager, USPCameraManager>(true);

            // Referential manager
            SimulatedReferentialManager = new Mock<IReferentialManager>();
            ClassLocator.Default.Register(() => SimulatedReferentialManager.Object);

            // PM User service
            ClassLocator.Default.Register(typeof(IPMUserService), typeof(PMUserService), true);
            ClassLocator.Default.Register<PMConfiguration>(
                () => PMConfiguration.Init(Path.Combine(Path.GetDirectoryName(PathHelper.GetExecutingAssemblyPath()),
                    PMConfigurationFilePath)), true);

            ClassLocator.Default.Register<IHardwareLoggerFactory>(() => new Mock<IHardwareLoggerFactory>().Object);

            // Forward logging from algo library
            // ILogger logger = new SerilogLogger<object>();
            // var _algorithmLoggerService = new AlgosLibrary.ManagedEventQueue();
            // var _eventForwarder = new EventForwarder(logger, "[ALGOS]");
            // _algorithmLoggerService.AddMessageEventHandler(_eventForwarder.ForwardEvent);
        }
    }
}
