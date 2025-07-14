using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Referentials.TestUtils.Positions
{
    public static class AnaPositionFactory
    {
        public static AnaPosition Build(Action<AnaPosition> action = null)
        {
            var position = new AnaPosition(new MotorReferential(),
                3,
                4,
                1,
                1,
                new List<ZPiezoPosition> { ZPiezoPositionFactory.Build(), ZPiezoPositionFactory.Build() }
            );
            action?.Invoke(position);
            return position;
        }
    }
}
