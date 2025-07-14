using System;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.CalibFlow
{
    // LiseHF Signal Calibration (Every wafer or after xx time) -- needed for calculation measre
    // ==> previously requires LiseHF Integration Time Calibration
    // ==> previously requires LiseHF Spot Calibration
    public class LiseHFCalibrationFlow : FlowComponent<CalibrationLiseHFInput, CalibrationLiseHFResult, LiseHFDarkRefConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly double _quality;

        public LiseHFCalibrationFlow(CalibrationLiseHFInput input) : base(input, "LiseHFCalibrationFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _quality = 0;
        }

        protected override void Process()
        {
            var probeLiseHF = HardwareUtils.GetProbeFromID<ProbeLiseHF>(_hardwareManager, Input.ProbeID);

            var liseHFCalibration = new LiseHFCalibration(probeLiseHF, Logger);

            var liseHFCalibParams = new LiseHFCalibParams();

            var liseHFInputParams = Input.InputParams;

            var calibResult = liseHFCalibration.DoCalibration(liseHFCalibParams, liseHFInputParams, CancellationToken);

            Result.CalibResult = calibResult;
            Result.Quality = _quality;
            Result.Timestamp = DateTime.Now;
        }

        public bool CheckCalibrationValidity()
        {
            if (_quality == 0)
            {
                return false;
            }

            return true;
        }
    }
}
