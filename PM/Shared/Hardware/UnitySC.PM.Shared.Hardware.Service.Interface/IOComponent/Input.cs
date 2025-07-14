using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent
{
    [XmlInclude(typeof(DigitalInput))]
    [XmlInclude(typeof(AnalogInput))]
    [KnownType(typeof(DigitalInput))]
    [KnownType(typeof(AnalogInput))]
    [DataContract]
    public class Input : IO
    {
    }

    public class DigitalInput : Input
    { }

    public class AnalogInput : Input
    { }
}
