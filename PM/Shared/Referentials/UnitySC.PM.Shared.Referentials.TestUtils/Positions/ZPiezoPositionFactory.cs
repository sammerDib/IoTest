using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Referentials.TestUtils.Positions
{
    public static class ZPiezoPositionFactory
    {
        public static ZPiezoPosition Build()
        {
            return new ZPiezoPosition(new StageReferential(), "axis#1", 1.2.Millimeters());
        }
    }
}
