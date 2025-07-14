using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class AutolightConfiguration : DefaultConfiguration
    {
        public double SaturationMax { get; set; } = 0.9;
    }
}
