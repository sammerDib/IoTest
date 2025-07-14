using System;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Referentials.TestUtils.Positions;

namespace UnitySC.PM.ANA.Service.Interface.TestUtils.Context
{
    public static class AnaPositionContextFactory
    {
        public static AnaPositionContext Build(Action<AnaPositionContext> action = null)
        {
            var context = new AnaPositionContext(AnaPositionFactory.Build());
            action?.Invoke(context);
            return context;
        }
    }
}
