using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.Metro.Topography
{
    public class DieStatExternalProcessing
    {
        public double Total;
        public int NbItems;
        public string Unit;
        public double Mean => NbItems > 0 ? Total / NbItems : double.NaN;
    }

    public class TopographyResult : MeasureResultBase
    {
        #region Properties

        [XmlAttribute("MeasureVersion")]
        [DataMember]
        public string MeasureVersion { get; set; } = "1.0.0";

        [DataMember]
        public TopographyResultSettings Settings { get; set; } = new TopographyResultSettings();

        [IgnoreDataMember]
        [XmlIgnore]
        public Dictionary<string, MetroDoubleStatsContainer> ExternalOutputStats { get; set; } = new Dictionary<string, MetroDoubleStatsContainer>();

        #endregion Properties

        #region Stats Generation

        public override List<ResultDataStats> GenerateStatisticsValues(long dbResId)
        {
            FillNonSerializedProperties(true, true);

            var dataStats = new List<ResultDataStats>();

            int takeStats = 3;

            if (ExternalOutputStats != null)
            {
                foreach (var externalOutputStat in ExternalOutputStats.Take(takeStats))
                {
                    dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, externalOutputStat.Key, externalOutputStat.Value.Mean, (int)UnitType.NoUnit));
                    dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, externalOutputStat.Key, externalOutputStat.Value.Min, (int)UnitType.NoUnit));
                    dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, externalOutputStat.Key, externalOutputStat.Value.Max, (int)UnitType.NoUnit));
                    dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, externalOutputStat.Key, (double)externalOutputStat.Value.State, (int)UnitType.NoUnit));
                }
            }

            return dataStats;
        }

        public override void FillNonSerializedProperties(bool generateState, bool generateStatistics)
        {
            if (Settings == null) return;

            // Stats
            var externalOutputsData = new Dictionary<string, LinkedList<MetroDoubleStatsContainer>>();
            var externalOutputsDictionary = Settings.ExternalProcessingOutputs?.ToDictionary(output => output.Name);

            if (generateStatistics)
            {
                QualityScore = 1.0;
            }

            void ComputeState(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is TopographyPointResult point)) return;

                foreach (var data in point.TopographyDatas)
                {
                    if (data == null || data.ExternalProcessingResults == null)
                        continue; // case of pattern rec fail for example

                    foreach (var externalProcessingResult in data.ExternalProcessingResults)
                    {
                        if (externalOutputsDictionary != null && externalOutputsDictionary.TryGetValue(externalProcessingResult.Name, out var associatedOutput))
                        {
                            externalProcessingResult.State = MeasureStateComputer.GetMeasureState(externalProcessingResult.Value, associatedOutput.OutputTolerance, associatedOutput.OutputTarget);
                        }
                        else
                        {
                            Console.WriteLine($"{nameof(TopographyResult)}.{nameof(FillNonSerializedProperties)} : Cannot deduce the state of the data because there is no output named {externalProcessingResult.Name} in current settings.");
                        }
                    }
                }
            }

            void ComputeStatistics(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is TopographyPointResult point)) return;

                point.GenerateStats();

                foreach (var externalProcessingStat in point.ExternalProcessingStats)
                {
                    if (!externalOutputsData.ContainsKey(externalProcessingStat.Key))
                    {
                        externalOutputsData.Add(externalProcessingStat.Key, new LinkedList<MetroDoubleStatsContainer>());
                    }

                    externalOutputsData[externalProcessingStat.Key].AddLast(externalProcessingStat.Value);
                }

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
                ExternalOutputStats = new Dictionary<string, MetroDoubleStatsContainer>();

                foreach (var externalOutputData in externalOutputsData)
                {
                    ExternalOutputStats.Add(externalOutputData.Key, MetroDoubleStatsContainer.GenerateFromStats(externalOutputData.Value));
                }
            }
        }

        #endregion Stats Generation

        #region CSV

        public override byte[] ToCsv()
        {
            bool isDieResult = (Dies != null) && (Dies.Count > 0);

            var lines = new List<string>();

            // Regular
            if (!isDieResult)
            {
                var measurePointResults = Points.OfType<TopographyPointResult>().ToList();
                bool hasRepeta = measurePointResults.Any(result => result.Datas.Count > 1);

                // Header
                var sbCSV = new CSVStringBuilder();
                AppendCSVAutomInfo(ref sbCSV);
                AppendStateAndPositionHeader(ref sbCSV);

                foreach (var outputStat in ExternalOutputStats)
                {
                    AppendHeader(outputStat.Key, hasRepeta, ref sbCSV);
                }
                sbCSV.RemoveEndDelim();
                lines.Add(sbCSV.ToString());

                // Values
                for (int i = 0; i < measurePointResults.Count; i++)
                {
                    sbCSV.Clear();

                    var p = measurePointResults[i];
                    sbCSV.Append($"{i + 1}", p.State.ToHumanizedString(), $"{p.WaferRelativeXPosition}", $"{p.WaferRelativeYPosition}");

                    foreach (var outputStat in ExternalOutputStats)
                    {
                        p.ExternalProcessingStats.TryGetValue(outputStat.Key, out var container);
                        AppendStats(container, hasRepeta, ref sbCSV);
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
                AppendDieStateAndPositionHeader(ref sbCSV);

                foreach (var outputStat in ExternalOutputStats)
                {
                    AppendHeader(outputStat.Key, hasRepeta, ref sbCSV);
                }
                sbCSV.RemoveEndDelim();
                lines.Add(sbCSV.ToString());

                int index = 1;
                foreach (var die in Dies)
                {
                    foreach (var p in die.Points.OfType<TopographyPointResult>())
                    {
                        sbCSV.Clear();

                        sbCSV.Append($"{index}", p.State.ToHumanizedString(),
                           $"{die.ColumnIndex}", $"{die.RowIndex}",
                           $"{p.WaferRelativeXPosition}", $"{p.WaferRelativeYPosition}",
                           $"{p.XPosition}", $"{p.YPosition}");

                        foreach (var outputStat in ExternalOutputStats)
                        {
                            p.ExternalProcessingStats.TryGetValue(outputStat.Key, out var container);
                            AppendStats(container, hasRepeta, ref sbCSV);
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
