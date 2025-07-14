using System.Collections.Generic;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck.Configuration
{
    public interface ISubstrateSlotChuckConfig<TSubstSlotConfig>
    {
        List<TSubstSlotConfig> SubstrateSlotConfigs { get; set; }
    }
}
