using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Execution
{
    public class AutofocusInputTest : IANAInputFlow
    {
        public ANAContextBase InitialContext { get; set; }

        public bool Error { get; set; }

        public InputValidity CheckInputValidity()
        {
            return new InputValidity(true);
        }
    }
}
