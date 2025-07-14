using System.Threading;

using UnitySC.PM.ANA.Service.Core.AlignmentMarks;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class AlignmentMarksFlowDummy : AlignmentMarksFlow
    {
        public AlignmentMarksFlowDummy(AlignmentMarksInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Thread.Sleep(1000);
            Result.Confidence = 1;
            Result.ShiftX = 3.Micrometers();
            Result.ShiftY = 2.Micrometers();
            Result.RotationAngle = new Angle(0.5, AngleUnit.Degree);
        }
    }
}
