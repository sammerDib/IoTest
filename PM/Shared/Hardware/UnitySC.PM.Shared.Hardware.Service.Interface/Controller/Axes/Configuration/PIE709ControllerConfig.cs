using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    [Serializable]
    [DataContract]
    public class PIE709ControllerConfig : PiezoControllerConfig
    {
        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public int BaudRate { get; set; }

        [DataMember]
        public int TriggerInLineId { get; set; }

        [DataMember]
        public int TriggerOutLineId { get; set; }

        [DataMember]
        public int WaveGeneratorId { get; set; }
    }
}
