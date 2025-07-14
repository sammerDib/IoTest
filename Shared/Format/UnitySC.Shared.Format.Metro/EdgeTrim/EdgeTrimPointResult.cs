using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnitySC.Shared.Format.Metro.Step;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.EdgeTrim
{
    [DataContract]
    public class EdgeTrimPointResult : MeasurePointResult
    {
        [XmlIgnore]
        public MetroStatsContainer HeightStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer WidthStat { get; private set; }

        [XmlIgnore]
        internal List<EdgeTrimPointData> EdgeTrimDatas => Datas?.OfType<EdgeTrimPointData>().ToList();

        // Angle related to wafer notch @ bottom, positive in anticlockwise direction (aka trigo)
        [XmlElement("ScanAngle")]
        [DataMember]
        public Angle ScanAngle { get; set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            var edgeTrimHeightData = new LinkedList<Tuple<Length, MeasureState>>();
            var edgeTrimWidthData = new LinkedList<Tuple<Length, MeasureState>>();
            var data = EdgeTrimDatas;

            if (data == null || data.Count == 0)
            {
                QualityScore = 0.0;
                return;
            }

            ComputeQualityScoreFromDatas();

            foreach (var pointData in data)
            {
                edgeTrimHeightData.AddLast(new Tuple<Length, MeasureState>(pointData.Height, pointData.HeightState));
                edgeTrimWidthData.AddLast(new Tuple<Length, MeasureState>(pointData.Width, pointData.WidthState));
            }

            HeightStat = MetroStatsContainer.GenerateFromLength(edgeTrimHeightData);
            WidthStat = MetroStatsContainer.GenerateFromLength(edgeTrimWidthData);

            ScanAngle = (Math.Atan2(YPosition, XPosition) + (Math.PI / 2.0)).Radians().ToUnit(AngleUnit.Degree);
        }

        public override List<ResultValue> GetResultValues()
        {
            var edgeTrimPointDatas = EdgeTrimDatas;
            if (edgeTrimPointDatas == null || edgeTrimPointDatas.Count == 0) return null;

            List<ResultValue> resultValues = null;
            if (edgeTrimPointDatas.Count > 1)
            {
                if (HeightStat == null || WidthStat == null)
                {
                    GenerateStats();
                }

                resultValues = FormatResultValues(HeightStat.Mean, WidthStat.Mean);
            }
            else
            {
                resultValues = FormatResultValues(edgeTrimPointDatas[0].Height, edgeTrimPointDatas[0].Width);
            }

            return resultValues;
        }

        private List<ResultValue> FormatResultValues(Length height, Length width)
        {
            var resultValues = new List<ResultValue>();
            if (!(height is null))
            {
                resultValues.Add(new ResultValue() { Name = "Height", Value = height.Value, Unit = height.UnitSymbol });
            }

            if (!(width is null))
            {
                resultValues.Add(new ResultValue() { Name = "Width", Value = width.Value, Unit = width.UnitSymbol });
            }

            return resultValues;
        }

        #endregion
    }
}
