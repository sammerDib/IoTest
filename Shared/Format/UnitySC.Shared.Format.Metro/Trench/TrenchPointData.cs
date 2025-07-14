using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Trench
{
    [DataContract]
    public class TrenchPointData : MeasureScanLine
    {
        [DataMember]
        public Length Depth { get; set; }

        [XmlIgnore]
        public MeasureState DepthState { get; set; } = MeasureState.NotMeasured;

        [DataMember]
        public Length Width { get; set; }

        [XmlIgnore]
        public MeasureState WidthState { get; set; } = MeasureState.NotMeasured;

        public override string ToString()
        {
             return $"{base.ToString()} Trench Depth:{Depth}, Width:{Width}";
        }
    }
}
