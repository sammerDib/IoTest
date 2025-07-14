using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Calibration;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Client.Proxy.Context;
using UnitySC.PM.ANA.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.ANA.Client.Proxy.Light;
using UnitySC.PM.ANA.Client.Proxy.Measure;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Proxy
{
    public class ServiceLocator
    {
        public static CamerasSupervisor CamerasSupervisor => ClassLocator.Default.GetInstance<CamerasSupervisor>();
        public static IKeyboardMouseHook KeyboardMouseHook => ClassLocator.Default.GetInstance<IKeyboardMouseHook>();

        public static AxesSupervisor AxesSupervisor => ClassLocator.Default.GetInstance<AxesSupervisor>();
        public static ChuckSupervisor ChuckSupervisor => ClassLocator.Default.GetInstance<ChuckSupervisor>();

        public static ProbesSupervisor ProbesSupervisor => ClassLocator.Default.GetInstance<ProbesSupervisor>();

        public static ANARecipeSupervisor ANARecipeSupervisor => ClassLocator.Default.GetInstance<ANARecipeSupervisor>();

        public static MeasureSupervisor MeasureSupervisor => ClassLocator.Default.GetInstance<MeasureSupervisor>();

        public static LightsSupervisor LightsSupervisor => ClassLocator.Default.GetInstance<LightsSupervisor>();

        public static AlgosSupervisor AlgosSupervisor => ClassLocator.Default.GetInstance<AlgosSupervisor>();
        public static ReferentialSupervisor ReferentialSupervisor => ClassLocator.Default.GetInstance<ReferentialSupervisor>();
        public static CalibrationSupervisor CalibrationSupervisor => ClassLocator.Default.GetInstance<CalibrationSupervisor>();
        public static ContextSupervisor ContextSupervisor => ClassLocator.Default.GetInstance<ContextSupervisor>();
        public static ControllersSupervisor ControllersSupervisor => ClassLocator.Default.GetInstance<ControllersSupervisor>();        
        public static GlobalStatusSupervisor GlobalStatusSupervisor=> ClassLocator.Default.GetInstance<GlobalStatusSupervisor>();
    }
}
