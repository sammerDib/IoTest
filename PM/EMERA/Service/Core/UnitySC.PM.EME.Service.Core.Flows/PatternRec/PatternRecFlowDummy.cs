using System.Threading;

using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.PatternRec
{
    public class PatternRecFlowDummy : PatternRecFlow
    {
        public PatternRecFlowDummy(PatternRecInput input, IEmeraCamera camera) : base(input, camera)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            Result.Confidence = 0.98;
            Result.ShiftX = new Length(15, LengthUnit.Micrometer); // This value is used in tests, do not change it
            Result.ShiftY = new Length(12, LengthUnit.Micrometer); // This value is used in tests, do not change it
            Logger.Information($"{LogHeader} Shift(µm) x: {Result.ShiftX} y: {Result.ShiftY}");
        }
    }
}
