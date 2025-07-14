using System.Threading;

using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class PatternRecFlowDummy : PatternRecFlow
    {
        public PatternRecFlowDummy(PatternRecInput input) : base(input)
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
