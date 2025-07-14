using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.Metro.Pillar
{
    public class PillarResult : MeasureResultBase
    {
        #region Properties

        [XmlAttribute("MeasureVersion")]
        [DataMember]
        public string MeasureVersion { get; set; } = "1.0.0";

        [DataMember]
        public PillarResultSettings Settings { get; set; } = new PillarResultSettings();

        [XmlIgnore]
        public MetroStatsContainer HeightStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer WidthStat { get; private set; }

        #endregion Properties

        #region Stats Generation

        public override List<ResultDataStats> GenerateStatisticsValues(long dbResId)
        {
            FillNonSerializedProperties(true, true);

            var dataStats = new List<ResultDataStats>();

            int takeStats = 3;

            if (HeightStat != null && HeightStat.Mean != null)
            {
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Height", HeightStat.Mean.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "Height", HeightStat.Min.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "Height", HeightStat.Max.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "Height", (double)HeightStat.State, (int)UnitType.NoUnit));
                takeStats--;
            }

            if (WidthStat != null && WidthStat.Mean != null)
            {
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Width", WidthStat.Mean.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "Width", WidthStat.Min.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "Width", WidthStat.Max.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "Width", (double)WidthStat.State, (int)UnitType.NoUnit));
                takeStats--;
            }
            return dataStats;
        }

        public override void FillNonSerializedProperties(bool generateState, bool generateStatistics)
        {
            if (Settings == null) return;

            // Stats
            var heightData = new LinkedList<MetroStatsContainer>();
            var heightTolerance = Settings.HeightTolerance;
            var heightTarget = Settings.HeightTarget;

            var widthData = new LinkedList<MetroStatsContainer>();
            var widthTolerance = Settings.WidthTolerance;
            var widthTarget = Settings.WidthTarget;

            if (generateStatistics)
            {
                QualityScore = 1.0;
            }

            void ComputeState(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is PillarPointResult point)) return;

                foreach (var data in point.PilarDatas)
                {
                    if (heightTarget != null && heightTolerance != null)
                        data.HeightState = MeasureStateComputer.GetMeasureState(data.Height, heightTolerance, heightTarget);

                    if (widthTarget != null && widthTolerance != null)
                        data.WidthState = MeasureStateComputer.GetMeasureState(data.Width, widthTolerance, widthTarget);
                }
            }

            void ComputeStatistics(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is PillarPointResult point)) return;

                point.GenerateStats();

                heightData.AddLast(point.HeightStat);
                widthData.AddLast(point.WidthStat);

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
                HeightStat = MetroStatsContainer.GenerateFromStats(heightData);
                WidthStat = MetroStatsContainer.GenerateFromStats(widthData);
            }
        }

        #endregion Stats Generation

        #region CSV

        public override byte[] ToCsv()
        {
            bool isDieResult = Dies.Count > 0;

            var lines = new List<string>();

            bool hasHeight = HeightStat != null && HeightStat.Mean != null;
            bool hasWidth = WidthStat != null && WidthStat.Mean != null;

            // Regular
            if (!isDieResult)
            {
                var measurePointResults = Points.OfType<PillarPointResult>().ToList();
                bool hasRepeta = measurePointResults.Any(result => result.Datas.Count > 1);

                // Header
                var sbCSV = new CSVStringBuilder();
                AppendCSVAutomInfo(ref sbCSV);
                AppendStateAndPositionHeader(ref sbCSV);
                if (hasHeight)
                {
                    AppendHeader("Height", hasRepeta, ref sbCSV, " (µm)");
                }
                if (hasWidth)
                {
                    AppendHeader("Width", hasRepeta, ref sbCSV, " (µm)");
                }
                sbCSV.RemoveEndDelim();
                
                lines.Add(sbCSV.ToString());

                // Values
                for (int i = 0; i < measurePointResults.Count; i++)
                {
                    sbCSV.Clear();

                    var p = measurePointResults[i];
                    sbCSV.Append($"{i + 1}", p.State.ToHumanizedString(), $"{p.XPosition}", $"{p.YPosition}");
                    if (hasHeight)
                    {
                        AppendStatsMicrometers(p.HeightStat, hasRepeta, ref sbCSV);
                    }
                    if (hasWidth)
                    {
                        AppendStatsMicrometers(p.WidthStat, hasRepeta, ref sbCSV);
                    }
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
                sbCSV.Append("No", "State", "Die IndexRepeta X (mm)", "Die IndexRepeta Y (mm)", "X (mm)", "Y (mm)");
                if (hasHeight)
                {
                    AppendHeader("Height", hasRepeta, ref sbCSV, " (µm)");
                }
                if (hasWidth)
                {
                    AppendHeader("Width", hasRepeta, ref sbCSV, " (µm)");
                }
                sbCSV.RemoveEndDelim();

                lines.Add(sbCSV.ToString());
                sbCSV.Clear();
               
                int index = 1;
                foreach (var die in Dies)
                {
                    foreach (var p in die.Points.OfType<PillarPointResult>())
                    {
                    	sbCSV.Clear();

                        sbCSV.Append($"{index}", p.State.ToHumanizedString(),
                            $"{die.ColumnIndex}", $"{die.RowIndex}",
                            $"{p.XPosition}",$"{p.YPosition}");

                        if (hasHeight)
                        {
                            AppendStatsMicrometers(p.HeightStat, hasRepeta, ref sbCSV);
                        }
                        if (hasWidth)
                        {
                            AppendStatsMicrometers(p.WidthStat, hasRepeta, ref sbCSV);
                        }

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
