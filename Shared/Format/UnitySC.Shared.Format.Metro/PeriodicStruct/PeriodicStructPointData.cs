using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.PeriodicStruct
{
    [DataContract]
    public class PeriodicStructPointData : MeasureScanLine
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

		[DataMember]
        public Length Pitch { get; set; }
		
		[DataMember]
        public int StructCount { get; set; }

#region Detailled DataStructByElement        
        [DataMember]
        public List<Length> StructHeights { get; set; } = new List<Length>();

		[DataMember]
        public List<Length> StructWidths { get; set; } = new List<Length>();
 #endregion

        public override string ToString()
        {
             return $"{base.ToString()} PeriodicStruct Height:{Height}, Width:{Width}, Pitch:{Pitch}, Count:{StructCount}";
        }
    }
}
