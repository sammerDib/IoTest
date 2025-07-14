using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Warp
{
    [DataContract]
    public class WarpPointData : MeasurePointDataResultBase, IMeasureAirGap
    {
        /// <summary>
        /// RPD (Reference Plane Deviation) is calculated at the end of the measure
        /// </summary>
        [DataMember]
        public Length RPD { get; set; }

        /// <summary>
        /// TotalThickness is set only if measure is done with Dual lise probe, or if wafer is transparent
        /// </summary>
        [DataMember]
        public Length TotalThickness { get; set; }

        /// <summary>
        /// AirGapUp is always set
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public Length AirGapUp { get; set; }

        /// <summary>
        /// AirGapDown is set only if measure, is done with Dual lise probe
        /// </summary>
        [XmlIgnore]
        [DataMember]
        public Length AirGapDown { get; set; }

        /// <summary>
        /// ZMedian is set only if measure is done with Dual lise probe, or if wafer is transparent
        /// </summary>
        [XmlIgnore]
        [DataMember]
        public Length ZMedian { get; set; }

        [XmlIgnore]
        [DataMember]
        public bool IsSurfaceWarp { get; set; } = false;

        /// <summary>
        /// X position of the measure point data
        /// </summary>
        [XmlIgnore]
        [DataMember]
        public double XPosition { get; set; }

        /// <summary>
        /// Y position of the measure point data
        /// </summary>
        [XmlIgnore]
        [DataMember]
        public double YPosition { get; set; }

        public override string ToString()
        {
             return $"{base.ToString()} RPD : {RPD}";
        }
    }
}
