using System.Threading;

using UnitySC.PM.ANA.Service.Core.WaferMap;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class DieAndStreetSizesFlowDummy : DieAndStreetSizesFlow
    {
        public DieAndStreetSizesFlowDummy(DieAndStreetSizesInput input) : base(input)
        {
        }

        protected override void Process()
        {
            Result.Confidence = 0.97;

            var dieWidth = (Input.BottomRightCorner.Position.X - Input.TopLeftCorner.Position.X).Millimeters();
            var dieHeight = (Input.TopLeftCorner.Position.Y - Input.BottomRightCorner.Position.Y).Millimeters();

            Result.DieDimensions = new DieDimensionalCharacteristic(
                                       dieWidth: dieWidth,
                                       dieHeight: dieHeight,
                                       streetWidth: dieWidth / 10,
                                       streetHeight: dieHeight / 15,
                                       dieAngle: 0.Degrees());
            Result.Status = new FlowStatus(FlowState.Success);
            Thread.Sleep(1000);
        }
    }
}
