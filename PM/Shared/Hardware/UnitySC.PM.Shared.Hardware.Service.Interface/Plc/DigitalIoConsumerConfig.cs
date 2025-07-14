using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Plc
{
    [DataContract]
    [Serializable]
    public class DigitalIoConsumerConfig
    {
        // The name of the output we need to trigger
        // The name should reflect the behaviour of the IO
        [XmlAttribute("Name")] public string Name { get; set; }

        // The name of the IoController which manages the output
        [XmlAttribute("ControllerId")] public string ControllerId { get; set; }
    }
}
