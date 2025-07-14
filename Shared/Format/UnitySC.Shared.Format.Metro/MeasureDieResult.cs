using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Format.Metro.TSV;

namespace UnitySC.Shared.Format.Metro
{
    [Serializable]
    [XmlInclude(typeof(TSVDieResult))]
    [DataContract]
    public class MeasureDieResult
    {
        [XmlAttribute("Col")]
        [DataMember]
        public int ColumnIndex { get; set; }

        [XmlAttribute("Row")]
        [DataMember]
        public int RowIndex { get; set; }

        [XmlArrayItem("Point")]
        [DataMember]
        public List<MeasurePointResult> Points { get; set; } = new List<MeasurePointResult>();

        [XmlAttribute("State")]
        [DataMember]
        public GlobalState State { get; set; }

        [XmlIgnore]
        [DataMember]
        public double WaferRelativeXPosition { get; private set; }

        [XmlIgnore]
        [DataMember]
        public double WaferRelativeYPosition { get; private set; }

        public void ComputeWaferRelativePosition(WaferMap waferMap)
        {
            WaferRelativeXPosition = waferMap.WaferRelativeXPositionLeftOfColumn(ColumnIndex);
            WaferRelativeYPosition = waferMap.WaferRelativeYPositionTopOfRow(RowIndex);
        }
    }
}
