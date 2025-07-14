using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Format.Metro
{
    [Serializable]
    [DataContract]
    public class ExternalProcessingResult
    {
        [XmlAttribute("Name")]
        [DataMember]
        public string Name { get; set; }

        [XmlAttribute("Value")]
        [DataMember]
        public double Value { get; set; }

        [XmlAttribute("Unit")]
        [DataMember]
        public string Unit { get; set; }

        [XmlIgnore]
        [DataMember]
        public MeasureState State { get; set; } = MeasureState.NotMeasured;

        public override string ToString()
        {
            string res;
            if (State == MeasureState.NotMeasured)
                res = $"Name: {Name} Not measured";
            else
                res = $"Name: {Name} Value: {Value} Unit: {Unit}";

            return res;
        }
    }
}
