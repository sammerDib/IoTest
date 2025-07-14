using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    [Serializable]
    [DataContract]
    public class LineMotObjectivesSelectorConfig : ObjectivesSelectorConfigBase
    {

        [DataMember]
        public string EthernetIP { get; set; }

        [DataMember]
        /// <summary>IP adress of the axis to move</summary>
        public string IpAddressAxis { get; set; }

        [DataMember]
        /// <summary>Port number of the axis to move</summary>
        public string PortNumberAxis { get; set; }

        [DataMember]
        /// <summary>Minimum position (mm)</summary>
        public Length PositionMin { get; set; }

        [DataMember]
        /// <summary>Maximum position (mm)</summary>
        public Length PositionMax { get; set; }

        [DataMember]
        /// <summary>Movement speed (m.s-1)</summary>
        public float Speed { get; set; }

        [DataMember]
        /// <summary>Movement acceleration (m.s-2)</summary>
        public float Accel { get; set; }

        [DataMember]
        /// <summary>Movement deceleration (m.s-2)</summary>
        public float Decel { get; set; }
    }
}
