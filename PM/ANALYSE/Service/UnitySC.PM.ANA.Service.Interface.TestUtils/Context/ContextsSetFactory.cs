using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.TestUtils.Context
{
    public static class ContextsSetFactory
    {
        public static ContextsList Build()
        {
            return new ContextsList(AnaPositionContextFactory.Build(), ObjectivesContextFactory.Build());
        }
    }
}
