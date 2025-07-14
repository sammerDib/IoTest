using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    [Serializable]
    [XmlInclude(typeof(USPChuckConfig<SubstrateSlotConfig>))]
    [XmlInclude(typeof(USPChuckConfig<SubstSlotWithPositionConfig>))]
    [DataContract]
    public class ChuckBaseConfig : DeviceBaseConfig
    {
        ///<summary>Used to know if the Chuck is, or isn't a vacuum Chuck</summary>
        [DataMember(Order = 20)]
        [Browsable(true), Category("Common"), DisplayName("Is Vacuum")]
        public bool IsVacuumChuck
        {
            get;
            set;
        }

        [DataMember]
        [Browsable(true), Category("Common")]
        public bool IsOpenChuck
        {
            get;
            set;
        }
        public virtual List<SubstrateSlotConfig> GetSubstrateSlotConfigs()
        {
            return null;
        }
        public virtual SubstrateSlotConfig GetSubstrateSlotConfigByWafer(Length waferDiameter)
        {
            return null;
        }
    }
}
