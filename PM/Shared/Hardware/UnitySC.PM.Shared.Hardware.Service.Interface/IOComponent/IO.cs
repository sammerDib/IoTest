using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent
{
    [XmlInclude(typeof(Output))]
    [XmlInclude(typeof(Input))]
    [KnownType(typeof(Output))]
    [KnownType(typeof(Input))]
    [DataContract]
    public class IO
    {
        [DataMember]
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [DataMember]
        public IOAddress Address { get; set; } = new IOAddress();

        [DataMember]
        public string CommandName { get; set; }


        [DataMember]
        public bool IsEnabled { get; set; }
    }

    public class IOAddress
    {
        [DataMember]
        [XmlAttribute("Module")]
        public int Module { get; set; }

        [DataMember]
        [XmlAttribute("Channel")]
        public int Channel { get; set; }
    }
}
