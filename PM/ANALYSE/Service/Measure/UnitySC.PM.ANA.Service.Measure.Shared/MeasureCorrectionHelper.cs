using System;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    public static class MeasureCorrectionHelper
    {
        public static Length ComputeAirGapUpDelta(CalibrationDualLiseFlowResult firstCalibration, CalibrationDualLiseFlowResult secondCalibration, DateTime timestamp)
        {
            // Compute time ratio over period between the two calibrations
            double timeRatio = (double)(timestamp.Ticks - firstCalibration.Timestamp.Ticks) / (double)((secondCalibration.Timestamp.Ticks - firstCalibration.Timestamp.Ticks)+double.Epsilon);

            // Compute airGap delta according measure timestamp
            return (secondCalibration.CalibResult.AirGapUp - firstCalibration.CalibResult.AirGapUp) * timeRatio;
        }

        public static Length ComputeAirGapDownDelta(CalibrationDualLiseFlowResult firstCalibration, CalibrationDualLiseFlowResult secondCalibration, DateTime timestamp)
        {
            // Compute time ratio over period between the two calibrations
            double timeRatio = (double)(timestamp.Ticks - firstCalibration.Timestamp.Ticks) / (double)(secondCalibration.Timestamp.Ticks - firstCalibration.Timestamp.Ticks);
            
            // Compute airGap delta according measure timestamp
            return (secondCalibration.CalibResult.AirGapDown - firstCalibration.CalibResult.AirGapDown) * timeRatio;
        }
    }
}
