using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Step
{
    // Properties without [DataMember] are not serialized
    [DataContract]
    public class StepPointResult : MeasurePointResult
    {
        [XmlIgnore]
        public MetroStatsContainer StepHeightStat { get; private set; }

        [XmlIgnore]
        internal List<StepPointData> StepDatas => Datas?.OfType<StepPointData>().ToList();

        // Angle related to wafer notch @ bottom, positive in anticlockwise direction (aka trigo)
        [XmlElement("ScanAngle")]
        [DataMember]
        public Angle ScanAngle { get; set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            var stepHeightData = new LinkedList<Tuple<Length, MeasureState>>();
            var data = StepDatas;

            if (data == null || data.Count == 0)
            {
                QualityScore = 0.0;
                return;
            }

            ComputeQualityScoreFromDatas();

            foreach (var pointData in data)
            {
                stepHeightData.AddLast(new Tuple<Length, MeasureState>(pointData.StepHeight, pointData.State));
            }

            StepHeightStat = MetroStatsContainer.GenerateFromLength(stepHeightData);
        }

        #endregion
    }
}
