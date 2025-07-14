using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.Metro.Step
{
    public class StepResult : MeasureResultBase
    {
        #region Properties

        [XmlAttribute("MeasureVersion")]
        [DataMember]
        public string MeasureVersion { get; set; } = "1.0.0";

        [DataMember]
        public StepResultSettings Settings { get; set; } = new StepResultSettings();

        [XmlIgnore]
        public MetroStatsContainer StepHeightStat { get; private set; }

        #endregion Properties

        #region Stats Generation

        public override List<ResultDataStats> GenerateStatisticsValues(long dbResId)
        {
            FillNonSerializedProperties(true, true);

            var dataStats = new List<ResultDataStats>();

            int takeStats = 3;

            if (StepHeightStat != null && StepHeightStat.Mean != null)
            {
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "StepHeight", StepHeightStat.Mean.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "StepHeight", StepHeightStat.Min.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "StepHeight", StepHeightStat.Max.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "StepHeight", (double)StepHeightStat.State, (int)UnitType.NoUnit));
                takeStats--;
            }

            return dataStats;
        }

        public override void FillNonSerializedProperties(bool generateState, bool generateStatistics)
        {
            if (Settings == null) return;

            // Stats
            var stepHeightData = new LinkedList<MetroStatsContainer>();

            var stepHeightTolerance = Settings.StepHeightTolerance;
            var stepHeightTarget = Settings.StepHeightTarget;

            if (generateStatistics)
            {
                QualityScore = 1.0;
            }

            void ComputeState(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is StepPointResult point)) return;

                foreach (var data in point.StepDatas)
                {
                    if (stepHeightTarget != null && stepHeightTolerance != null)
                        data.State = MeasureStateComputer.GetMeasureState(data.StepHeight, stepHeightTolerance, stepHeightTarget);
                }
            }

            void ComputeStatistics(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is StepPointResult point)) return;

                point.GenerateStats();

                stepHeightData.AddLast(point.StepHeightStat);

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
                StepHeightStat = MetroStatsContainer.GenerateFromStats(stepHeightData);
            }
        }

        #endregion Stats Generation

        #region CSV

        public override byte[] ToCsv()
        {
            bool isDieResult = Dies.Count > 0;

            var lines = new List<string>();

            bool hasStepHeight = StepHeightStat != null && StepHeightStat.Mean != null;

            // Regular
            if (!isDieResult)
            {
                var measurePointResults = Points.OfType<StepPointResult>().ToList();
                bool hasRepeta = measurePointResults.Any(result => result.Datas.Count > 1);

                // Header
                var sbCSV = new CSVStringBuilder();
                AppendCSVAutomInfo(ref sbCSV);
                AppendStateAndPositionHeader(ref sbCSV);
                if (hasStepHeight)
                {
                    AppendHeader("Step Height", hasRepeta, ref sbCSV, " (µm)");
                }
                sbCSV.RemoveEndDelim();
                
                lines.Add(sbCSV.ToString());

                // Values
                for (int i = 0; i < measurePointResults.Count; i++)
                {
                    sbCSV.Clear();

                    var p = measurePointResults[i];
                    sbCSV.Append($"{i + 1}", p.State.ToHumanizedString(), $"{p.XPosition}", $"{p.YPosition}");
                    if (hasStepHeight)
                    {
                        AppendStatsMicrometers(p.StepHeightStat, hasRepeta, ref sbCSV);
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
                if (hasStepHeight)
                {
                    AppendHeader("Step Height", hasRepeta, ref sbCSV, " (µm)");
                }
                sbCSV.RemoveEndDelim();

                lines.Add(sbCSV.ToString());
                sbCSV.Clear();
               
                int index = 1;
                foreach (var die in Dies)
                {
                    foreach (var p in die.Points.OfType<StepPointResult>())
                    {
                    	sbCSV.Clear();

                        sbCSV.Append($"{index}", p.State.ToHumanizedString(),
                            $"{die.ColumnIndex}", $"{die.RowIndex}",
                            $"{p.XPosition}",$"{p.YPosition}");

                        if (hasStepHeight)
                        {
                            AppendStatsMicrometers(p.StepHeightStat, hasRepeta, ref sbCSV);
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
