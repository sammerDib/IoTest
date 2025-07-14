using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface
{
    public enum CommunicationMode
    {
        USB,
        Ethernet,
        Com,
    }

    public class SpectrometerAvantesControllerConfig : ControllerConfig
    {
        [DataMember]
        public CommunicationMode CommunicationMode { get; set; }

        [DataMember]
        public string SerialNumber { get; set; }

        [DataMember]
        public short Port { get; set; }
    }
}
