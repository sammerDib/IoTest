using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Data
{
    [DataContract]
    public class DataAttribute
    {
        public DataAttribute()
        {

        }
        public DataAttribute(string name, AttributeType type, string identifier, string controllerName, int module = 0, int channel = 0)
        {
            Name = name;
            Type = type;
            Identifier = identifier;
            ControllerName = controllerName;
            Module = module;
            Channel = channel;
        }

        [DataMember]
        public string ControllerName { get; set; }

        [DataMember]
        public int Module { get; set; }

        [DataMember]
        public int Channel { get; set; }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string CommandName { get; set; }

        [DataMember]
        public AttributeType Type { get; set; }

        [DataMember]
        public string Identifier { get; set; }

        [DataMember]
        public object Value { get; set; }

        [DataMember]
        public bool DigitalValue { get; set; }

        [DataMember]
        public double AnalogValue { get; set; }

        public bool Changed { get; set; }
    }
}
