using System;
using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Hardware.Chuck
{
    [Serializable]
    [DataContract]
    public class WaferSlotConfiguration
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public Length Diameter
        {
            get;
            set;
        }

        [DataMember]
        public Length XTheoricalPosition
        {
            get;
            set;
        }

        [DataMember]
        public Length YTheoricalPosition
        {
            get;
            set;
        }

    }
}
