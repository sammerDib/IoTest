using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.EdgeTrim
{
    [DataContract]
    public class EdgeTrimPointData : MeasureScanLine
    {
        [DataMember]
        public Length Height { get; set; }

        [XmlIgnore]
        public MeasureState HeightState { get; set; } = MeasureState.NotMeasured;

        [DataMember]
        public Length Width { get; set; }

        [XmlIgnore]
        public MeasureState WidthState { get; set; } = MeasureState.NotMeasured;

        public override string ToString()
        {
            return $"{base.ToString()} Height: {Height} Width: {Width}";
        }
    }
}
