using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Pillar
{   // Properties without [DataMember] are not serialized
    [DataContract]
    public class PillarPointResult : MeasurePointResult
    {
        [XmlIgnore]
        public MetroStatsContainer HeightStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer WidthStat { get; private set; }

        [XmlIgnore]
        internal List<PillarPointData> PilarDatas => Datas?.OfType<PillarPointData>().ToList();

        // Angle related to wafer notch @ bottom, positive in anticlockwise direction (aka trigo)
        [XmlElement("ScanAngle")]
        [DataMember]
        public Angle ScanAngle { get; set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            var heightData = new LinkedList<Tuple<Length, MeasureState>>();
            var widthData = new LinkedList<Tuple<Length, MeasureState>>();
            var data = PilarDatas;

            if (data == null || data.Count == 0)
            {
                QualityScore = 0.0;
                return;
            }

            ComputeQualityScoreFromDatas();

            foreach (var pointData in data)
            {
                heightData.AddLast(new Tuple<Length, MeasureState>(pointData.Height, pointData.HeightState));
                widthData.AddLast(new Tuple<Length, MeasureState>(pointData.Width, pointData.WidthState));
            }

            HeightStat = MetroStatsContainer.GenerateFromLength(heightData);
            WidthStat = MetroStatsContainer.GenerateFromLength(widthData);
        }

        #endregion
    }
}
