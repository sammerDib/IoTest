using System.Collections.Generic;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.Shared.Flow.Implementation
{
    public interface IFlowsConfiguration
    {
        List<FlowConfigurationBase> Flows { get; set; }
    }
}
