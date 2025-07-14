using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class AutofocusConfiguration : DefaultConfiguration
    {
        public AFLiseConfiguration AFLise { get; set; } = new AFLiseConfiguration();

        public AFCameraConfiguration AFCamera { get; set; } = new AFCameraConfiguration();
    }
}
