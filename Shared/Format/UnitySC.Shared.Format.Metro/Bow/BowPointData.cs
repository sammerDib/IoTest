using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Bow
{
    [DataContract]
    public class BowPointData : MeasurePointDataResultBase
    {
        [DataMember]
        public Length AirGap { get; set; }

        [XmlIgnore]
        [DataMember]
        public double XPosition { get; set; }

        [XmlIgnore]
        [DataMember]
        public double YPosition { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()} AirGap : {AirGap}";
        }
    }
}
