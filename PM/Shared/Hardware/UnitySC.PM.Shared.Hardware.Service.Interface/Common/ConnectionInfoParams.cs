using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Common
{
    public enum SerialFlowControl
    {
        None = 0,
        XonXoff,
        RtsCts,
        DsrDts
    }

    public enum SerialParity
    {
        None = 0,
        Odd,
        Even,
        Mark,
        Space
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class EthernetCom
    {
        [DataMember(Order = 5)]
        [DisplayName("IP"), Category("Communication Info"), Browsable(true)]
        public string IP { get; set; }

        [DataMember(Order = 10)]
        [DisplayName("Port"), Category("Communication Info"), Browsable(true)]
        public int Port { get; set; }
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class SerialCom
    {
        [DataMember]
        public string Port { get; set; }

        [DataMember]
        public int BaudRate { get; set; }

        [DataMember]
        public string DataFrame { get; set; }

        [DataMember]
        public SerialFlowControl FlowControl { get; set; }
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class OpcCom
    {
        [DataMember]
        public string Hostname { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public string RootNodeId;

        [DataMember]
        public string DeviceNodeID { get; set; }
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class Ethercat
    {
        [DataMember]
        public int Address { get; set; }
    }
}
