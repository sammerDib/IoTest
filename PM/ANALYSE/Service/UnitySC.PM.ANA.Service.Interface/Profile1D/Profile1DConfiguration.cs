using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Profile1D
{
    public class Profile1DConfiguration : DefaultConfiguration
    {
        public int MotionTimeout { get; set; } = int.MaxValue;
    }
}
