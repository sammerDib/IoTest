using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck.Configuration;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;



namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    [Serializable]    
    [XmlInclude(typeof(DummyUSPChuckConfig))]
    [XmlInclude(typeof(NICouplerChuckConfig))]
    [XmlInclude(typeof(DMTChuckConfig))]
    [XmlInclude(typeof(EMEChuckConfig))]
    [XmlInclude(typeof(ANAChuckConfig))]
    [DataContract]
    public class USPChuckConfig<TSubstSlotConfig> : ChuckBaseConfig, ISubstrateSlotChuckConfig<TSubstSlotConfig>
                          where TSubstSlotConfig : SubstrateSlotConfig
    {
        [DataMember]
        public List<TSubstSlotConfig> SubstrateSlotConfigs { get; set; }

        public override List<SubstrateSlotConfig> GetSubstrateSlotConfigs()
        {
            return SubstrateSlotConfigs.Cast<SubstrateSlotConfig>().ToList();
        }
        public override SubstrateSlotConfig GetSubstrateSlotConfigByWafer(Length waferDiameter)
        {
            return SubstrateSlotConfigs.Cast<SubstrateSlotConfig>().Find(x => x.Diameter == waferDiameter);
        }
    }
}
