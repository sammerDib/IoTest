using System;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Referentials.TestUtils.Positions
{
    public static class XYZTopZBottomPositionFactory
    {
        public static XYZTopZBottomPosition Build(Action<XYZTopZBottomPosition> action = null)
        {
            var position = new XYZTopZBottomPosition(new MotorReferential(),
                4,
                3,
                1,
                2
            );
            action?.Invoke(position);
            return position;
        }
    }
}
