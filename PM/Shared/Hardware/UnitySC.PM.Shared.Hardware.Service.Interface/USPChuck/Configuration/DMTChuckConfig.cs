using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    [Serializable]
    [DataContract]
    public class DMTChuckConfig : USPChuckConfig<SubstrateSlotConfig>
    {
        // Only one slot on PSD chuck
        public SubstrateSlotConfig GetSlotConfig()
        {
            if ((SubstrateSlotConfigs[0] is SubstrateSlotConfig substConfig)) // Check null and get a variable if not null             
                return substConfig;
            else
                return null;
        }
    }
}
