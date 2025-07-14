using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.Metro.TSV
{
    [Serializable]
    [XmlInclude(typeof(TSVDieResult))]
    [KnownType(typeof(TSVDieResult))]
    [DataContract]
    public class TSVResult : MeasureResultBase
    {
        [XmlAttribute("MeasureVersion")]
        [DataMember]
        public string MeasureVersion { get; set; } = "1.0.0";

        [XmlElement("Copla")]
        [DataMember]
        public BestFitPlan BestFitPlan { get; set; }

        [DataMember]
        public TSVResultSettings Settings { get; set; } = new TSVResultSettings();

        [XmlIgnore]
        public MetroStatsContainer LengthTsvStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer WidthTsvStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer DepthTsvStat { get; private set; }

        #region Stats Generation

        public override void FillNonSerializedProperties(bool generateState, bool generateStatistics)
        {
            if (Settings == null || Settings.LengthTarget == null || Settings.WidthTarget == null || Settings.DepthTarget == null) return;

            // Stats
            var lengthData = new LinkedList<MetroStatsContainer>();
            var widthData = new LinkedList<MetroStatsContainer>();
            var depthData = new LinkedList<MetroStatsContainer>();

            if (generateStatistics)
            {
                QualityScore = 1.0;
            }

            void ComputeState(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is TSVPointResult tsvPoint)) return;

                foreach (var tsvPointData in tsvPoint.TSVDatas)
                {
                    tsvPointData.LengthState = MeasureStateComputer.GetMeasureState(tsvPointData.Length, Settings.LengthTolerance, Settings.LengthTarget);
                    tsvPointData.WidthState = MeasureStateComputer.GetMeasureState(tsvPointData.Width, Settings.WidthTolerance, Settings.WidthTarget);
                    tsvPointData.DepthState = MeasureStateComputer.GetMeasureState(tsvPointData.Depth, Settings.DepthTolerance, Settings.DepthTarget);
                }
            }

            void ComputeStatistics(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is TSVPointResult tsvPoint)) return;

                tsvPoint.GenerateStats();

                lengthData.AddLast(tsvPoint.LengthTsvStat);
                widthData.AddLast(tsvPoint.WidthTsvStat);
                depthData.AddLast(tsvPoint.DepthTsvStat);

                if (tsvPoint.QualityScore < QualityScore)
                {
                    QualityScore = tsvPoint.QualityScore;
                }
            }

            if (Points != null)
            {
                foreach (var measureResultPoint in Points)
                {
                    measureResultPoint.ComputeWaferRelativePosition();

                    if (generateState)
                    {
                        ComputeState(measureResultPoint);
                    }

                    if (generateStatistics)
                    {
                        ComputeStatistics(measureResultPoint);
                    }
                }
            }

            if (DiesMap != null)
            {
                double dieHeight = DiesMap.DieSizeHeight.Millimeters;

                foreach (var dieResult in Dies)
                {
                    dieResult.ComputeWaferRelativePosition(DiesMap);

                    foreach (var measurePointResult in dieResult.Points)
                    {
                        measurePointResult.ComputeWaferRelativePosition(dieResult.WaferRelativeXPosition, dieResult.WaferRelativeYPosition, dieHeight);

                        if (generateState)
                        {
                            ComputeState(measurePointResult);
                        }

                        if (generateStatistics)
                        {
                            ComputeStatistics(measurePointResult);
                        }
                    }
                }
            }

            if (generateStatistics)
            {
                LengthTsvStat = MetroStatsContainer.GenerateFromStats(lengthData);
                WidthTsvStat = MetroStatsContainer.GenerateFromStats(widthData);
                DepthTsvStat = MetroStatsContainer.GenerateFromStats(depthData);
            }
        }

        public override List<ResultDataStats> GenerateStatisticsValues(long dbResId)
        {
            FillNonSerializedProperties(true, true);
            double dDefault = 0.0;
            return new List<ResultDataStats>
            {
                new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Length", LengthTsvStat.Mean?.Micrometers ?? dDefault, (int)UnitType.um),
                new ResultDataStats(dbResId, (int)ResultValueType.Min, "Length", LengthTsvStat.Min?.Micrometers ?? dDefault, (int)UnitType.um),
                new ResultDataStats(dbResId, (int)ResultValueType.Max, "Length", LengthTsvStat.Max?.Micrometers ?? dDefault, (int)UnitType.um),
                new ResultDataStats(dbResId, (int)ResultValueType.State, "Length", (double)LengthTsvStat.State, (int)UnitType.NoUnit),
                new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Width", WidthTsvStat.Mean?.Micrometers ?? dDefault, (int)UnitType.um),
                new ResultDataStats(dbResId, (int)ResultValueType.Min, "Width", WidthTsvStat.Min?.Micrometers ?? dDefault, (int)UnitType.um),
                new ResultDataStats(dbResId, (int)ResultValueType.Max, "Width", WidthTsvStat.Max?.Micrometers ?? dDefault, (int)UnitType.um),
                new ResultDataStats(dbResId, (int)ResultValueType.State, "Width", (double)WidthTsvStat.State, (int)UnitType.NoUnit),
                new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Depth", DepthTsvStat.Mean?.Micrometers ?? dDefault, (int)UnitType.um),
                new ResultDataStats(dbResId, (int)ResultValueType.Min, "Depth", DepthTsvStat.Min?.Micrometers ?? dDefault, (int)UnitType.um),
                new ResultDataStats(dbResId, (int)ResultValueType.Max, "Depth", DepthTsvStat.Max?.Micrometers ?? dDefault, (int)UnitType.um),
                new ResultDataStats(dbResId, (int)ResultValueType.State, "Depth", (double)DepthTsvStat.State, (int)UnitType.NoUnit)
            };
        }

        #endregion Stats Generation

        public override byte[] ToCsv()
        {
            bool isDieResult = (Dies?.Count ?? -1) > 0;

            var lines = new List<string>();

            // Regular
            if (!isDieResult)
            {
                var measurePointResults = Points.OfType<TSVPointResult>().ToList();
                bool hasRepeta = measurePointResults.Any(result => result.Datas.Count > 1);

                // Header
                var sbCSV = new CSVStringBuilder();
                AppendCSVAutomInfo(ref sbCSV);
                AppendStateAndPositionHeader(ref sbCSV);
                AppendHeader("Depth", hasRepeta, ref sbCSV, " (µm)");
                AppendHeader("Width", hasRepeta, ref sbCSV, " (µm)");
                AppendHeader("Length", hasRepeta, ref sbCSV, " (µm)");
                sbCSV.Add_NoDelim("Copla (µm)");

                lines.Add(sbCSV.ToString());
              
                for (int i = 0; i < measurePointResults.Count; i++)
                {
                    sbCSV.Clear();

                    var p = measurePointResults[i];
                    sbCSV.Append($"{i + 1}", p.State.ToHumanizedString(), $"{p.WaferRelativeXPosition}", $"{p.WaferRelativeYPosition}");
                    AppendStatsMicrometers(p.DepthTsvStat, hasRepeta, ref sbCSV);
                    AppendStatsMicrometers(p.WidthTsvStat, hasRepeta, ref sbCSV);
                    AppendStatsMicrometers(p.LengthTsvStat, hasRepeta, ref sbCSV);
                    sbCSV.Add_NoDelim($"{p.CoplaInWaferValue?.Micrometers}");
                    lines.Add(sbCSV.ToString());
                }
            }
            // Dies
            else
            {
                bool hasRepeta = Dies.SelectMany(result => result.Points).Any(result => result.Datas.Count > 1);

                var sbCSV = new CSVStringBuilder();
                AppendCSVAutomInfo(ref sbCSV);
                AppendDieStateAndPositionHeader(ref sbCSV);
                AppendHeader("Depth", hasRepeta, ref sbCSV, " (µm)");
                AppendHeader("Width", hasRepeta, ref sbCSV, " (µm)");
                AppendHeader("Length", hasRepeta, ref sbCSV, " (µm)");
                sbCSV.Add_NoDelim("Copla (µm)");

                lines.Add(sbCSV.ToString());

                int index = 1;
                foreach (var die in Dies)
                {
                    foreach (var p in die.Points.OfType<TSVPointResult>())
                    {
                        sbCSV.Clear();

                        sbCSV.Append($"{index}", p.State.ToHumanizedString(),
                           $"{die.ColumnIndex}", $"{die.RowIndex}",
                           $"{p.WaferRelativeXPosition}", $"{p.WaferRelativeYPosition}",
                           $"{p.XPosition}", $"{p.YPosition}");

                        AppendStatsMicrometers(p.DepthTsvStat, hasRepeta, ref sbCSV);
                        AppendStatsMicrometers(p.WidthTsvStat, hasRepeta, ref sbCSV);
                        AppendStatsMicrometers(p.LengthTsvStat, hasRepeta, ref sbCSV);
                        sbCSV.Add_NoDelim($"{p.CoplaInDieValue?.Micrometers}");

                        lines.Add(sbCSV.ToString());
                        index++;
                    }
                }
            }

            byte[] preamble = Encoding.UTF8.GetPreamble();
            string csvContent = string.Join(Environment.NewLine, lines);
            byte[] bytes = Encoding.UTF8.GetBytes(csvContent);
            return preamble.Concat(bytes).ToArray();
        }
    }
}
