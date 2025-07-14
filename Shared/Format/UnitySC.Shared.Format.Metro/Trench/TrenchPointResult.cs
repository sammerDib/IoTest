using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Trench
{
    // Properties without [DataMember] are not serialized
    [DataContract]
    public class TrenchPointResult : MeasurePointResult
    {
        [XmlIgnore]
        public MetroStatsContainer DepthStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer WidthStat { get; private set; }

        [XmlIgnore]
        internal List<TrenchPointData> TrenchDatas => Datas?.OfType<TrenchPointData>().ToList();

        // Angle related to wafer notch @ bottom, positive in anticlockwise direction (aka trigo)
        [XmlElement("ScanAngle")]
        [DataMember]
        public Angle ScanAngle { get; set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            var depthData = new LinkedList<Tuple<Length, MeasureState>>();
            var widthData = new LinkedList<Tuple<Length, MeasureState>>();
            var data = TrenchDatas;

            if (data == null || data.Count == 0)
            {
                QualityScore = 0.0;
                return;
            }

            ComputeQualityScoreFromDatas();

            foreach (var pointData in data)
            {
                depthData.AddLast(new Tuple<Length, MeasureState>(pointData.Depth, pointData.DepthState));
                widthData.AddLast(new Tuple<Length, MeasureState>(pointData.Width, pointData.WidthState));
            }

            DepthStat = MetroStatsContainer.GenerateFromLength(depthData);
            WidthStat = MetroStatsContainer.GenerateFromLength(widthData);
        }

        #endregion
    }
}
