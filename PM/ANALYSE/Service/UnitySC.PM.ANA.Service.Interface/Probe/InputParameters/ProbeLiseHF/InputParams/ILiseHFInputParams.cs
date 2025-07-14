using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface ILiseHFInputParams : IProbeInputParams
    {
        Length DepthTarget { get; set; }
        LengthTolerance DepthTolerance { get; set; }

        bool IsLowIlluminationPower { get; set; }
        double IntegrationTimems { get; set; } //Only use to put integration time from calibration in case of pooling (StartContinuousAcquisition)
        double IntensityFactor { get; set; }
        int NbMeasuresAverage { get; set; }
        double Threshold  { get; set; }
        double ThresholdPeak { get; set; }

        CalibrationFrequency CalibrationFreq { get; set; }
        bool SaveFFTSignal { get; set; }

    }

    
}
