using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.Metro.NanoTopo
{
    public class NanoTopoResult : MeasureResultBase
    {
        #region Properties

        [XmlAttribute("MeasureVersion")]
        [DataMember]
        public string MeasureVersion { get; set; } = "1.0.0";

        [DataMember]
        public NanoTopoResultSettings Settings { get; set; } = new NanoTopoResultSettings();

        [XmlIgnore]
        public MetroStatsContainer RoughnessStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer StepHeightStat { get; private set; }

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

            if (RoughnessStat != null && RoughnessStat.Mean != null)
            {
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Roughness", RoughnessStat.Mean.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "Roughness", RoughnessStat.Min.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "Roughness", RoughnessStat.Max.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "Roughness", (double)RoughnessStat.State, (int)UnitType.NoUnit));
                takeStats--;
            }

            if (StepHeightStat != null && StepHeightStat.Mean != null)
            {
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "StepHeight", StepHeightStat.Mean.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "StepHeight", StepHeightStat.Min.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "StepHeight", StepHeightStat.Max.Nanometers, (int)UnitType.nm));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "StepHeight", (double)StepHeightStat.State, (int)UnitType.NoUnit));
                takeStats--;
            }

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
            var roughnessData = new LinkedList<MetroStatsContainer>();
            var stepHeightData = new LinkedList<MetroStatsContainer>();
            var externalOutputsData = new Dictionary<string, LinkedList<MetroDoubleStatsContainer>>();

            var roughnessTolerance = Settings.RoughnessTolerance;
            var roughnessTarget = Settings.RoughnessTarget;
            var stepHeightTolerance = Settings.StepHeightTolerance;
            var stepHeightTarget = Settings.StepHeightTarget;
            var externalOutputsDictionary = Settings.ExternalProcessingOutputs?.ToDictionary(output => output?.Name);

            if (generateStatistics)
            {
                QualityScore = 1.0;
            }

            void ComputeState(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is NanoTopoPointResult point)) return;

                foreach (var data in point.NanoTopoDatas)
                {
                    if (roughnessTarget != null && roughnessTolerance != null)
                        data.RoughnessState = MeasureStateComputer.GetMeasureState(data.Roughness, roughnessTolerance, roughnessTarget);

                    if (stepHeightTarget != null && stepHeightTolerance != null)
                        data.StepHeightState = MeasureStateComputer.GetMeasureState(data.StepHeight, stepHeightTolerance, stepHeightTarget);

                    if (data?.ExternalProcessingResults != null && externalOutputsDictionary != null)
                    {
                        foreach (var externalProcessingResult in data?.ExternalProcessingResults)
                        {
                            if (externalOutputsDictionary.TryGetValue(externalProcessingResult.Name, out var associatedOutput))
                            {
                                externalProcessingResult.State = MeasureStateComputer.GetMeasureState(externalProcessingResult.Value, associatedOutput.OutputTolerance, associatedOutput.OutputTarget);
                            }
                            else
                            {
                                Console.WriteLine($"{nameof(NanoTopoResult)}.{nameof(FillNonSerializedProperties)} : Cannot deduce the state of the data because there is no output named {externalProcessingResult.Name} in current settings.");
                            }
                        }
                    }
                }
            }

            void ComputeStatistics(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is NanoTopoPointResult point)) return;

                point.GenerateStats();

                roughnessData.AddLast(point.RoughnessStat);
                stepHeightData.AddLast(point.StepHeightStat);

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
                RoughnessStat = MetroStatsContainer.GenerateFromStats(roughnessData);
                StepHeightStat = MetroStatsContainer.GenerateFromStats(stepHeightData);

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
            bool isDieResult = Dies?.Count > 0;
            var lines = new List<string>();
            bool hasRoughness = RoughnessStat != null && RoughnessStat.Mean != null;
            bool hasStepHeight = StepHeightStat != null && StepHeightStat.Mean != null;

            // Regular
            if (!isDieResult)
            {
                var measurePointResults = Points.OfType<NanoTopoPointResult>().ToList();
                bool hasRepeta = measurePointResults.Any(result => result.Datas.Count > 1);

                // Header
                var sbCSV = new CSVStringBuilder();
                AppendCSVAutomInfo(ref sbCSV);
                AppendStateAndPositionHeader(ref sbCSV);

                if (hasRoughness)
                {
                    AppendHeader("Roughness", hasRepeta, ref sbCSV, " (nm)");
                }

                if (hasStepHeight)
                {
                    AppendHeader("Step Height", hasRepeta, ref sbCSV, " (nm)");
                }

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

                    if (hasRoughness)
                    {
                        AppendStatsNanometers(p.RoughnessStat, hasRepeta, ref sbCSV);
                    }

                    if (hasStepHeight)
                    {
                        AppendStatsNanometers(p.StepHeightStat, hasRepeta, ref sbCSV);
                    }

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
                if (hasRoughness)
                {
                    AppendHeader("Roughness", hasRepeta, ref sbCSV, " (nm)");
                }

                if (hasStepHeight)
                {
                    AppendHeader("Step Height", hasRepeta, ref sbCSV, " (nm)");
                }

                foreach (var outputStat in ExternalOutputStats)
                {
                    AppendHeader(outputStat.Key, hasRepeta, ref sbCSV);
                }
                sbCSV.RemoveEndDelim();
                lines.Add(sbCSV.ToString());

                int index = 1;
                foreach (var die in Dies)
                {
                    foreach (var p in die.Points.OfType<NanoTopoPointResult>())
                    {
                        sbCSV.Clear();

                        sbCSV.Append($"{index}", p.State.ToHumanizedString(),
                           $"{die.ColumnIndex}", $"{die.RowIndex}",
                           $"{p.WaferRelativeXPosition}", $"{p.WaferRelativeYPosition}",
                           $"{p.XPosition}", $"{p.YPosition}");

                        if (hasRoughness)
                        {
                            AppendStatsNanometers(p.RoughnessStat, hasRepeta, ref sbCSV);
                        }

                        if (hasStepHeight)
                        {
                            AppendStatsNanometers(p.StepHeightStat, hasRepeta, ref sbCSV);
                        }

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
