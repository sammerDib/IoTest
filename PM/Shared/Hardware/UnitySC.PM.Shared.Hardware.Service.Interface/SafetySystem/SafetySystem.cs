using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.SafetySystem
{
    [Serializable]
    [DataContract]
    public class SafetySystem
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public uint SafetySystemID { get; set; }

        public string State { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SafetySystems
    {
        [DataMember]
        [Browsable(false)]
        public List<SafetySystem> SafetySystemsControl { get; set; }
    }
}
