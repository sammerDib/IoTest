using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IDualLiseCalibParams : IProbeCalibParams
    {
        OpticalReferenceDefinition ProbeCalibrationReference { get; set; }
        int NbRepeatCalib { get; set; }
        double TopLiseAirgapThreshold { get; set; }
        double BottomLiseAirgapThreshold { get; set; }
        double ZTopUsedForCalib { get; set; }
        double ZBottomUsedForCalib { get; set; }
        int CalibrationMode { get; set; }
    }
}
