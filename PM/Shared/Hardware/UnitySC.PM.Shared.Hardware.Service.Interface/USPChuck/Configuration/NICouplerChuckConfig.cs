using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    [Serializable]
    [DataContract]
    public class NICouplerChuckConfig : USPChuckConfig<SubstrateSlotConfig>
    {
        #region Properties

        [DataMember]
        public string Vacuum1 { get; set; }

        [DataMember]
        public string Vacuum2 { get; set; }

        [DataMember]
        public string VacuumValve1 { get; set; }

        [DataMember]
        public string VacuumValve2 { get; set; }

        [DataMember]
        public int StabilisationTime_ms { get; set; }

        [DataMember]
        [Browsable(false)]
        public List<WaferClampConfig> WaferClampList
        {
            get;
            set;
        }
        #endregion Properties      
    }
}
