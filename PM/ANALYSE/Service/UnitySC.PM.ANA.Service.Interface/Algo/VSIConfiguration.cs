using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class VSIConfiguration : DefaultConfiguration
    {
        public Length LambdaCenter { get; set; } = 621.Nanometers();// algo VSI Prm : wavelength of the light source
        public Length FwhmLambda { get; set; } = 124.Nanometers();  // algo VSI Prm : Spectral Bandwith of the light source - Full width at half maximum (fwhm)
        public double NoiseLevel { get; set; } = 0.5;               // algo VSI Prm : typical sensor noise standard deviation in LSB
        public double MaskThreshold { get; set; } = 0.0;           // algo VSI Prm : [0.0 .. 1.0] Percentage Mask Threshold (0 = No mask)
        public double PiezoMinStabilisationTime_ms { get; set; } = 30;  // Minimum waiting time between two piezo steps
        public Length PiezoPositionStabilisationAccuracy { get; set; } = 10.Nanometers();  // Expected piezo positioning resolution
    }
}
