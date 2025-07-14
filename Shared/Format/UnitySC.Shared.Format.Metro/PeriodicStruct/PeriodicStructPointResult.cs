using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.PeriodicStruct
{
    // Properties without [DataMember] are not serialized
    [DataContract]
    public class PeriodicStructPointResult : MeasurePointResult
    {
        [XmlIgnore]
        public MetroStatsContainer HeightStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer WidthStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer PitchStat { get; private set; }

        [XmlIgnore]
        public MetroDoubleStatsContainer CountStat { get; private set; }

        [XmlIgnore]
        internal List<PeriodicStructPointData> PeriodicStructDatas => Datas?.OfType<PeriodicStructPointData>().ToList();

        // Angle related to wafer notch @ bottom, positive in anticlockwise direction (aka trigo)
        [XmlElement("ScanAngle")]
        [DataMember]
        public Angle ScanAngle { get; set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            var heightData = new LinkedList<Tuple<Length, MeasureState>>();
            var widthData = new LinkedList<Tuple<Length, MeasureState>>();

            var pitchData = new LinkedList<Tuple<Length, MeasureState>>();
            var countData = new LinkedList<Tuple<double, MeasureState>>();

            var data = PeriodicStructDatas;

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

                pitchData.AddLast(new Tuple<Length, MeasureState>(pointData.Pitch, pointData.WidthState));
                countData.AddLast(new Tuple<double, MeasureState>((double)pointData.StructCount, pointData.WidthState));
            }

            HeightStat = MetroStatsContainer.GenerateFromLength(heightData);
            WidthStat = MetroStatsContainer.GenerateFromLength(widthData);

            PitchStat = MetroStatsContainer.GenerateFromLength(pitchData);
            CountStat = MetroDoubleStatsContainer.GenerateFromDoubles(countData, "");
        }

        #endregion Overrides of MeasurePointResult
    }
}
