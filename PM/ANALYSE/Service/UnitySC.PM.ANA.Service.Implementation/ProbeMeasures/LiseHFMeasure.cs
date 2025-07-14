using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Service.Implementation.ProbeMeasures
{
    public class LiseHFMeasure : IProbeMeasure
    {
        private LiseHFCalibration _liseHFCalibration;
        private ILogger _logger;
        private ProbeLiseHF _probeLiseHF;

        public LiseHFMeasure(ProbeLiseHF probeLiseHF, LiseHFCalibration liseHFCalibration, ILogger logger)
        {
            _probeLiseHF = probeLiseHF;
            _liseHFCalibration = liseHFCalibration;
            _logger = logger;
        }

        public IProbeResult DoMeasure(IProbeInputParams inputParameters)
        {
            return _probeLiseHF.DoMeasure(inputParameters);
        }
    }
}
