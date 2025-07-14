using System.Threading;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.AxisOrthogonality
{
    public class AxisOrthogonalityFlowDummy : AxisOrthogonalityFlow
    {
        public AxisOrthogonalityFlowDummy(AxisOrthogonalityInput input, IEmeraCamera camera) : base(input, camera)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            Result.XAngle = new Angle(0.1, AngleUnit.Degree);
            Result.YAngle = new Angle(0.1, AngleUnit.Degree);
            Logger.Information($"XAngle {Result.XAngle}");
            Logger.Information($"YAngle {Result.YAngle}");
        }
    }
}
