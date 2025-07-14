using System.Threading;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Service.Interface
{
    public interface IProbeCalibrationManager
    {
        CancellationToken CancellationToken { get; set; }

        IProbeCalibResult GetCalibration(bool createIfNeeded, string probeId, IProbeInputParams probeInputParams, PointPosition point, DieIndex die = null);

        void SetCalibration(string probeId, IProbeCalibResult probeCalibResult, IProbeInputParams probeInputParams, PointPosition point, DieIndex die = null);

        void ResetCalibrations();

        void RecipeExecutionStarted();

        void MeasureExecutionTerminated();

        void CorrectMeasurePoint(MeasurePointDataResultBase measurePointDataResult);
    }
}
