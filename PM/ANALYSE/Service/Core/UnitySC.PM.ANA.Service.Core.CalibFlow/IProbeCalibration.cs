using UnitySC.PM.ANA.Service.Interface;

namespace UnitySC.PM.ANA.Service.Core.CalibFlow
{
    public interface IProbeCalibration
    {
        IProbe Probe { get; set; }
        
        bool StartCalibration(IProbeCalibParams probeCalibParams, IProbeInputParams inputParameters);

        void CancelCalibration();
    }
}
