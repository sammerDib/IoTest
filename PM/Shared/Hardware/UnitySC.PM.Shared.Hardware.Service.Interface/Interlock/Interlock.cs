using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Interlock
{
    [Serializable]
    [DataContract]
    public class Interlock
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public uint InterlockID { get; set; }

        public string State { get; set; }
    }

    [Serializable]
    [DataContract]
    public class Interlocks
    {
        [DataMember]
        [Browsable(false)]
        public List<Interlock> InterlockPanels { get; set; }
    }
}
