using System.Collections.Generic;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Dataflow.Shared.Configuration
{
    public interface IDFClientConfiguration
    {
        string ExternalUserControlsDir { get; set; }
        int ToolKey { get; set; }
        List<ActorType> AvailableModules { get; set; }
    }
}
