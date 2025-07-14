using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.Metro.Bow
{
    public class BowResult : MeasureResultBase
    {
        #region Properties

        [XmlAttribute("MeasureVersion")]
        [DataMember]
        public string MeasureVersion { get; set; } = "1.0.0";

        [DataMember]
        public BowResultSettings Settings { get; set; } = new BowResultSettings();

        [XmlIgnore]
        public MetroStatsContainer BowStat { get; private set; }

        #endregion Properties

        #region Stats Generation

        public override List<ResultDataStats> GenerateStatisticsValues(long dbResId)
        {
            FillNonSerializedProperties(true, true);

            var dataStats = new List<ResultDataStats>();

            if (BowStat != null && BowStat.Mean != null)
            {
                // NOTE RTI pour TLA voir si on peux prendre l'unit de la target comme reférence au lieu de fixé Micrometers comem standard !

                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Bow", BowStat.Mean.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "Bow", BowStat.Min.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "Bow", BowStat.Max.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "Bow", (double)BowStat.State, (int)UnitType.NoUnit));
            }

            return dataStats;
        }

        public override void FillNonSerializedProperties(bool generateState, bool generateStatistics)
        {
            if (Settings == null) return;
          
            // Stats
            var bowData = new LinkedList<MetroStatsContainer>();

            var bowTargetMax = Settings.BowTargetMax;
            var bowTargetMin = Settings.BowTargetMin;
           
            if (generateStatistics)
            {
                QualityScore = 1.0;
            }

            void ComputeState(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is BowPointResult point)) return;

                foreach (var data in point.BowTotalPointDatas)
                {
                    data.State = MeasureStateComputer.GetMeasureState(data.Bow, bowTargetMin, bowTargetMax);
                }
            }

            void ComputeStatistics(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is BowPointResult point)) return;

                point.GenerateStats();

                bowData.AddLast(point.BowStat);

                if (point.QualityScore < QualityScore)
                {
                    QualityScore = point.QualityScore;
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
                BowStat = MetroStatsContainer.GenerateFromStats(bowData);
            }
        }

        #endregion Stats Generation

        #region CSV

        public override byte[] ToCsv()
        {
            bool isDieResult = Dies?.Count > 0;

            var lines = new List<string>();
       
            // Regular
            if (!isDieResult)
            {
                var measurePointResults = Points.OfType<BowPointResult>().ToList();
                bool hasRepeta = measurePointResults.Any(result => result.Datas.Count > 1);

                // Header
                var sbCSV = new CSVStringBuilder();
                AppendCSVAutomInfo(ref sbCSV);
                AppendStateAndPositionHeader(ref sbCSV);
                AppendHeader("Bow", hasRepeta, ref sbCSV, " (µm)");
                sbCSV.RemoveEndDelim();
                
                lines.Add(sbCSV.ToString());

                // Values
                for (int i = 0; i < measurePointResults.Count; i++)
                {
                    sbCSV.Clear();

                    var p = measurePointResults[i];
                    sbCSV.Append($"{i + 1}", p.State.ToHumanizedString(), $"{p.WaferRelativeXPosition}", $"{p.WaferRelativeYPosition}");

                    AppendStatsMicrometers(p.BowStat, hasRepeta, ref sbCSV);
                    sbCSV.RemoveEndDelim();
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
                AppendHeader("Bow", hasRepeta, ref sbCSV, " (µm)");
                sbCSV.RemoveEndDelim();

                lines.Add(sbCSV.ToString());
                sbCSV.Clear();

                int index = 1;
                foreach (var die in Dies)
                {
                    foreach (var p in die.Points.OfType<BowPointResult>())
                    {
                        sbCSV.Clear();

                        sbCSV.Append($"{index}", p.State.ToHumanizedString(),
                            $"{die.ColumnIndex}", $"{die.RowIndex}",
                            $"{p.WaferRelativeXPosition}", $"{p.WaferRelativeYPosition}",
                            $"{p.XPosition}",$"{p.YPosition}");

                        AppendStatsMicrometers(p.BowStat, hasRepeta, ref sbCSV);

                        sbCSV.RemoveEndDelim();
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

        #endregion CSV
    }
}
