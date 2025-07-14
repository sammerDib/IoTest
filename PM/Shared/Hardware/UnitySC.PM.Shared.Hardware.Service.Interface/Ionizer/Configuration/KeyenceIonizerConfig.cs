using System;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Hardware.Service.Interface.SafetySystem;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer
{
    [Serializable]
    [DataContract]
    public class KeyenceIonizerConfig : IonizerConfig
    {
        [DataMember]
        public SafetySystems SafetySystems { get; set; }
    }
}
