using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Pillar
{
    [DataContract]
    public class PillarPointData : MeasureScanLine
    {
        [DataMember]
        public Length Height { get; set; }

        [XmlIgnore]
        [DataMember]
        public MeasureState HeightState { get; set; } = MeasureState.NotMeasured;

        [DataMember]
        public Length Width { get; set; }

        [XmlIgnore]
        [DataMember]
        public MeasureState WidthState { get; set; } = MeasureState.NotMeasured;

        public override string ToString()
        {
             return $"{base.ToString()} Pillar Height:{Height}, Width:{Width}";
        }
    }
}
