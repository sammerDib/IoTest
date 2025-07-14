using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent
{
    [XmlInclude(typeof(DigitalOutput))]
    [XmlInclude(typeof(AnalogOutput))]
    [KnownType(typeof(DigitalOutput))]
    [KnownType(typeof(AnalogOutput))]
    [DataContract]
    public class Output : IO
    {
    }

    public class DigitalOutput : Output
    { }

    public class AnalogOutput : Output
    { }
}
