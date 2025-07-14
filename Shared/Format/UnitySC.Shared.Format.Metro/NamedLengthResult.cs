using System;
using UnitySC.Shared.Tools.Units;

using System.Xml.Serialization;
using System.Runtime.Serialization;

using UnitySC.Shared.Format.Metro.Thickness;

namespace UnitySC.Shared.Format.Metro
{
    [Serializable]
    [XmlInclude(typeof(ThicknessLengthResult))]
    [KnownType(typeof(ThicknessLengthResult))]
    [DataContract]
    public class NamedLengthResult
    {
        [XmlAttribute("Name")]
        [DataMember]
        public string Name { get; set; }

        [XmlElement("Length")]
        [DataMember]
        public Length Length { get; set; }

        [XmlIgnore]
        [DataMember]
        public MeasureState State { get; set; } = MeasureState.NotMeasured;

        public override string ToString()
        {
            return $"Name: {Name} Length: {Length}";
        }
    }
}
