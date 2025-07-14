using System.IO;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Referentials;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Implementation;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.PM.ANA.Service.Shared.TestUtils.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
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

namespace UnitySC.PM.ANA.Service.Core.Console.Test
{
    public static class Bootstrapper
    {
        public static object BootstrapperLock = new object();
        public static bool IsRegister { get; private set; }

        public static void Register()
        {
            lock (BootstrapperLock)
            {
                if (!IsRegister)
                {
                    // Configuration
                    var currentConfiguration = new FakeConfigurationManager();

                    // Init logger
                    SerilogInit.Init(currentConfiguration.LogConfigurationFilePath);
                    ClassLocator.Default.Register<IPMServiceConfigurationManager>(() => currentConfiguration, true);

                    // Logger with caller name
                    ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

                    // Logger without caller name
                    ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

                    // PM Configuration
                    ClassLocator.Default.Register<PMConfiguration>(() => PMConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().PMConfigurationFilePath), true);

                    // Flow Configuration
                    ClassLocator.Default.Register<IFlowsConfiguration>(() => FlowsConfiguration.Init(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>()), true);

                    // Calibration manager
                    ClassLocator.Default.Register<CalibrationManager>(() => new CalibrationManager(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath), true);
                    ClassLocator.Default.Register<IReferentialManager, AnaReferentialManager>(true);

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

                    // Camera service
                    ClassLocator.Default.Register(typeof(ICameraServiceEx), typeof(CameraServiceEx), true);
                    ClassLocator.Default.Register(typeof(ICameraManager), typeof(USPCameraManager), true);

                    // PM User service
                    ClassLocator.Default.Register(typeof(IPMUserService), typeof(PMUserService), true);

                    // Calibration Service
                    ClassLocator.Default.Register(typeof(ICalibrationService), typeof(CalibrationService), true);

                    // Calibration Service
                    ClassLocator.Default.Register(typeof(IAlgoService), typeof(AlgoService), true);

                    // Calibration manager
                    ClassLocator.Default.Register<CalibrationManager>(true);

                    IsRegister = true;
                }
            }
        }
    }
}
