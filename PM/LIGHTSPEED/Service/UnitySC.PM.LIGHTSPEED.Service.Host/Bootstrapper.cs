using UnitySC.PM.LIGHTSPEED.Service.Implementation;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.AttenuationFilter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface.FiberSwitch;
using UnitySC.PM.Shared.Hardware.Service.Interface.Global;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface.Mppc;
using UnitySC.PM.Shared.Hardware.Service.Interface.OpticalPowermeter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.PolarisationFilter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Hardware.Service.Interface.FastAttenuation;
using UnitySC.PM.Shared.Status.Service.Implementation;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.LIGHTSPEED.Service.Host
{
    public static class Bootstrapper
    {
        public static void Register()
        {
            // Logger with caller name
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));

            // Message
            ClassLocator.Default.Register<GalaSoft.MvvmLight.Messaging.IMessenger, GalaSoft.MvvmLight.Messaging.Messenger>(true);

            // Global device service
            ClassLocator.Default.Register(typeof(IGlobalDeviceService), typeof(GlobalDeviceService), singleton: true);

            // Hardware service
            ClassLocator.Default.Register(typeof(IPlcService), typeof(PlcService), singleton: true);
            ClassLocator.Default.Register(typeof(IAttenuationFilterService), typeof(AttenuationFilterService), singleton: true);
            ClassLocator.Default.Register(typeof(IPolarisationFilterService), typeof(PolarisationFilterService), singleton: true);
            ClassLocator.Default.Register(typeof(IDistanceSensorService), typeof(DistanceSensorService), singleton: true);
            ClassLocator.Default.Register(typeof(IFiberSwitchService), typeof(FiberSwitchService), singleton: true);
            ClassLocator.Default.Register(typeof(ILaserService), typeof(LaserService), singleton: true);
            ClassLocator.Default.Register(typeof(IMppcService), typeof(MppcService), singleton: true);
            ClassLocator.Default.Register(typeof(IOpticalPowermeterService), typeof(OpticalPowermeterService), singleton: true);
            ClassLocator.Default.Register(typeof(IShutterService), typeof(ShutterService), singleton: true);
            ClassLocator.Default.Register(typeof(IChamberService), typeof(ChamberService), singleton: true);
            ClassLocator.Default.Register(typeof(IFastAttenuationService), typeof(FastAttenuationService), singleton: true);

            // Hardware manager
            ClassLocator.Default.Register(typeof(HardwareManager), typeof(HardwareManager), singleton: true);

            // global status service
            ClassLocator.Default.Register<GlobalStatusService>(true);
            ClassLocator.Default.Register<IGlobalStatusServer>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());
            ClassLocator.Default.Register<IGlobalStatusService>(() => ClassLocator.Default.GetInstance<GlobalStatusService>());

            ClassLocator.Default.Register(typeof(IAcquisitionService), typeof(LSAcquisitionService), singleton: true);
            ClassLocator.Default.Register(typeof(IRotatorsKitCalibrationService), typeof(LSRotatorsKitCalibrationService), singleton: true);
            ClassLocator.Default.Register(typeof(ILiseHFService), typeof(NSTLiseHFService), singleton: true);
            ClassLocator.Default.Register(typeof(IFeedbackLoopService), typeof(LSFeedbackLoopService), singleton: true);
        }
    }
}
