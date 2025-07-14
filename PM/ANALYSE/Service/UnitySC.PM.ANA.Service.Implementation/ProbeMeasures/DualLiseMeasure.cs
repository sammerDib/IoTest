using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Service.Implementation.ProbeMeasures
{
    public class DualLiseMeasure : IProbeMeasure
    {

        private DualLiseCalibration _dualLiseCalibration;
        private ILogger _logger;
        private ProbeDualLise _probeDualLise;

        public DualLiseMeasure(ProbeDualLise probeDualLise, DualLiseCalibration dualLiseCalibration, ILogger logger)
        {
            _probeDualLise=probeDualLise;
            _dualLiseCalibration=dualLiseCalibration;
            _logger=logger;
        }

        private string FormatMessage(string deviceID, string message)
        {
            return $"[{deviceID}]{message}";
        }

        public IProbeResult DoMeasure(IProbeInputParams inputParameters)
        {
            return new DualLiseResult();
        }
    }
}
