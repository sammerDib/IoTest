using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class PSIConfiguration : DefaultConfiguration
    {
        public double PiezoMinStabilisationTime_ms { get; set; } = 500;  // Minimum waiting time between two piezo steps
        public Length PiezoPositionStabilisationAccuracy { get; set; } = 10.Nanometers();  // Expected piezo positioning resolution
    }
}
