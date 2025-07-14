using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Core.Context.Test
{
    internal class UnknownContext : ANAContextBase
    {
    }

    internal class ContextWithSubcontexts : ANAContextBase
    {
        public XYPositionContext PositionContext { get; set; }
    }

    internal class ContextWithNotOnlySubcontexts : ANAContextBase
    {
        public XYPositionContext PositionContext { get; set; }

        public int IAmNotASubcontext { get; set; }
    }
}
