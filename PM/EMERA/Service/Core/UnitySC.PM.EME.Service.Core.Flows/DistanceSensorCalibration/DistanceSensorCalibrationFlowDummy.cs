using System.Threading;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.DistanceSensorCalibration
{
    public class DistanceSensorCalibrationFlowDummy : DistanceSensorCalibrationFlow
    {
        public DistanceSensorCalibrationFlowDummy(DistanceSensorCalibrationInput input, IEmeraCamera camera) : base(input, camera)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            Result.OffsetX = 10.Millimeters();
            Result.OffsetY = 50.Millimeters();
            Logger.Information($"Distance Sensor Offset X : {Result.OffsetX}");
            Logger.Information($"Distance Sensor Offset Y : {Result.OffsetY}");
        }
    }
}
