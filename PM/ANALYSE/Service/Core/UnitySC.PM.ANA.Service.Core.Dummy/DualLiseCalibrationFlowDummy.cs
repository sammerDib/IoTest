using System;

using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class DualLiseCalibrationFlowDummy : DualLiseCalibrationFlow

    {
        public DualLiseCalibrationFlowDummy(CalibrationDualLiseInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Result.CalibResult = new ProbeDualLiseCalibResult()
            {
                AirGapUp = 100.Micrometers(),
                AirGapDown = 100.Micrometers(),
                ZTopUsedForCalib = 0,
                ZBottomUsedForCalib = 0,
                GlobalDistance = 750.Micrometers()
            };
            Result.Timestamp = DateTime.Now;
            Result.Quality = 1;
        }
    }
}
