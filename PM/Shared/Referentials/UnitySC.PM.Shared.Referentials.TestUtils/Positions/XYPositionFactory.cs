using System;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Referentials.TestUtils.Positions
{
    public static class XYPositionFactory
    {
        public static XYPosition Build(Action<XYPosition> action = null)
        {
            var position = new XYPosition(new MotorReferential(), 4, 3);
            action?.Invoke(position);
            return position;
        }
    }
}
