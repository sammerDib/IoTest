using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Thickness
{
    [DataContract]
    public class ThicknessResult : MeasureResultBase
    {
        #region Properties

        [XmlAttribute("MeasureVersion")]
        [DataMember]
        public string MeasureVersion { get; set; } = "1.0.2";

        [DataMember]
        public ThicknessResultSettings Settings { get; set; } = new ThicknessResultSettings();

        [XmlIgnore]
        public Dictionary<string, MetroStatsContainer> ThicknessLayerStats { get; set; }

        [XmlIgnore]
        public MetroStatsContainer TotalThicknessStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer WaferThicknessStat { get; private set; }

        [DataMember]
        public List<Length> WarpWaferResults { get; set; } = new List<Length>();

        [XmlIgnore]
        public MetroStatsContainer WarpStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer RPDStat { get; private set; }

        #endregion Properties

        #region Stats Generation

        public override List<ResultDataStats> GenerateStatisticsValues(long dbResId)
        {
            FillNonSerializedProperties(true, true);

            var dataStats = new List<ResultDataStats>();

            int takeStats = 5;

            if (TotalThicknessStat != null && TotalThicknessStat.Mean != null)
            {
                // NOTE RTI pour TLA voir si on peux prendre l'unit de la target comme reférence au lieu de fixé Micrometers comem standard !

                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Total", TotalThicknessStat.Mean.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "Total", TotalThicknessStat.Min.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "Total", TotalThicknessStat.Max.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "Total", (double)TotalThicknessStat.State, (int)UnitType.NoUnit));
                takeStats--;
            }

            if (WaferThicknessStat != null && WaferThicknessStat.Mean != null)
            {
                // NOTE RTI pour TLA voir si on peux prendre l'unit de la target comme reférence au lieu de fixé Micrometers comem standard !

                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "WaferThickness", WaferThicknessStat.Mean.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "WaferThickness", WaferThicknessStat.Min.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "WaferThickness", WaferThicknessStat.Max.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "WaferThickness", (double)WaferThicknessStat.State, (int)UnitType.NoUnit));
                takeStats--;
            }

            if (WarpStat != null && WarpStat.Mean != null)
            {
                // NOTE RTI pour TLA voir si on peux prendre l'unit de la target comme reférence au lieu de fixé Micrometers comem standard !

                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Warp", WarpStat.Mean.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "Warp", WarpStat.Min.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "Warp", WarpStat.Max.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "Warp", (double)WarpStat.State, (int)UnitType.NoUnit));
                takeStats--;
            }

            if (ThicknessLayerStats != null)
            {
                foreach (var thicknessStat in ThicknessLayerStats.Take(takeStats))
                {
                    // NOTE RTI pour TLA voir si on peux prendre l'unit de la target comme reférence au lieu de fixé Micrometers comem standard !

                    dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, thicknessStat.Key, thicknessStat.Value.Mean.Micrometers, (int)UnitType.um));
                    dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, thicknessStat.Key, thicknessStat.Value.Min.Micrometers, (int)UnitType.um));
                    dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, thicknessStat.Key, thicknessStat.Value.Max.Micrometers, (int)UnitType.um));
                    dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, thicknessStat.Key, (double)thicknessStat.Value.State, (int)UnitType.NoUnit));
                }
            }

            return dataStats;
        }

        public override void FillNonSerializedProperties(bool generateState, bool generateStatistics)
        {
            if (Settings == null) return;
            Settings.ComputeNotMeasuredLayers();

            // Stats
            var totalThicknessData = new LinkedList<MetroStatsContainer>();
            var waferThicknessData = new LinkedList<MetroStatsContainer>();
            var layersThicknessData = new Dictionary<string, LinkedList<MetroStatsContainer>>();

            var warpData = new LinkedList<MetroStatsContainer>();
            var RPDData = new LinkedList<MetroStatsContainer>();

            var totalTolerance = Settings.TotalTolerance;
            var totalTarget = Settings.TotalTarget;
            var thicknessLayersDictionary = Settings.ThicknessLayers?.ToDictionary(output => output.Name);

            if (generateStatistics)
            {
                QualityScore = 1.0;
            }

            void ComputeState(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is ThicknessPointResult point)) return;

                foreach (var data in point.ThicknessDatas)
                {
                    if (totalTarget != null && totalTolerance != null)
                    {
                        data.ComputeTotalThickness(Settings);
                        data.TotalState = MeasureStateComputer.GetMeasureState(data.TotalThickness, totalTolerance, totalTarget);

                        if (Settings.HasWaferThicknesss && data.WaferThicknessResult != null)
                        {
                            data.WaferThicknessResult.State = MeasureStateComputer.GetMeasureState(data.WaferThicknessResult.Length, totalTolerance, totalTarget);
                        }
                    }

                    if (thicknessLayersDictionary.IsNullOrEmpty()) continue;

                    foreach (var layerThicknessResult in data.ThicknessLayerResults)
                    {
                        if (thicknessLayersDictionary.TryGetValue(layerThicknessResult.Name, out var associatedLayer))
                        {
                            layerThicknessResult.State = MeasureStateComputer.GetMeasureState(layerThicknessResult.Length, associatedLayer.Tolerance, associatedLayer.Target);
                        }
                        else
                        {
                            Console.WriteLine($"{nameof(ThicknessResult)}.{nameof(FillNonSerializedProperties)} : Cannot deduce the state of the data because there is no output named {layerThicknessResult.Name} in current settings.");
                        }
                    }
                }

                if (Settings.HasWarpMeasure && Settings.WarpTargetMax != null)
                {
                    foreach (var data in point.RPDDatas)
                    {
                        data.State = MeasureStateComputer.GetMeasureState_NoLimit(data.RPD);
                    }
                }
            }

            void ComputeStatistics(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is ThicknessPointResult point)) return;

                point.GenerateStats();

                totalThicknessData.AddLast(point.TotalThicknessStat);

                if(Settings.HasWarpMeasure)
                    RPDData.AddLast(point.RPDStat);


                if (point.WaferThicknessStat != null)
                {
                    waferThicknessData.AddLast(point.WaferThicknessStat);
                }

                foreach (var layerthicknessStat in point.ThicknessLayerStats)
                {
                    if (!layersThicknessData.ContainsKey(layerthicknessStat.Key))
                    {
                        layersThicknessData.Add(layerthicknessStat.Key, new LinkedList<MetroStatsContainer>());
                    }

                    layersThicknessData[layerthicknessStat.Key].AddLast(layerthicknessStat.Value);
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


                if (generateStatistics && Settings.HasWarpMeasure && WarpWaferResults != null && WarpWaferResults.Count > 0 && Settings.WarpTargetMax != null)
                {
                    var waferWarpData = new LinkedList<Tuple<Length, MeasureState>>();
                    WarpWaferResults.ForEach(w => waferWarpData.AddLast(new Tuple<Length, MeasureState>(w, MeasureStateComputer.GetMeasureState(w, new Length(0d, LengthUnit.Micrometer), Settings.WarpTargetMax))));
                    WarpStat = waferWarpData.Any() ? MetroStatsContainer.GenerateFromLength(waferWarpData) : MetroStatsContainer.Empty;
                    warpData.AddLast(WarpStat);
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
                TotalThicknessStat = MetroStatsContainer.GenerateFromStats(totalThicknessData);

                if (Settings.HasWaferThicknesss && waferThicknessData.Any())
                {
                    WaferThicknessStat = MetroStatsContainer.GenerateFromStats(waferThicknessData);

                    if (Settings.HasWarpMeasure )
                    {
                        if(warpData.Any())
                            WarpStat = MetroStatsContainer.GenerateFromStats(warpData);

                        if(RPDData.Any())
                            RPDStat = MetroStatsContainer.GenerateFromStats(RPDData);

                    }
                }

                ThicknessLayerStats = new Dictionary<string, MetroStatsContainer>();

                foreach (var layerData in layersThicknessData)
                {
                    ThicknessLayerStats.Add(layerData.Key, MetroStatsContainer.GenerateFromStats(layerData.Value));
                }
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
                var measurePointResults = Points.OfType<ThicknessPointResult>().ToList();
                bool hasRepeta = measurePointResults.Any(result => result.Datas.Count > 1);

                // Header
                var sbCSV = new CSVStringBuilder();
                AppendCSVAutomInfo(ref sbCSV);
                AppendStateAndPositionHeader(ref sbCSV);

                AppendHeader("Total", hasRepeta, ref sbCSV, " (nm)");

                if (WaferThicknessStat != null)
                {
                    AppendHeader("Wafer Thickness", hasRepeta, ref sbCSV, " (nm)");

                    if (WarpStat != null)
                    {
                        AppendHeader("Warp", hasRepeta, ref sbCSV, " (nm)");
                    }
                }

                foreach (var layerStat in ThicknessLayerStats)
                {
                    AppendHeader(layerStat.Key, hasRepeta, ref sbCSV, " (nm)");
                }

                sbCSV.RemoveEndDelim();
                lines.Add(sbCSV.ToString());

                // Values
                for (int i = 0; i < measurePointResults.Count; i++)
                {
                    sbCSV.Clear();

                    var p = measurePointResults[i];
                    sbCSV.Append($"{i + 1}", p.State.ToHumanizedString(), $"{p.WaferRelativeXPosition}", $"{p.WaferRelativeYPosition}");

                    AppendStatsNanometers(p.TotalThicknessStat, hasRepeta, ref sbCSV);
                    if (p.WaferThicknessStat?.Mean != null)
                    {
                        AppendStatsNanometers(p.WaferThicknessStat, hasRepeta, ref sbCSV);
                    }
                    foreach (var layerStat in ThicknessLayerStats)
                    {
                        p.ThicknessLayerStats.TryGetValue(layerStat.Key, out var container);
                        AppendStatsNanometers(container, hasRepeta, ref sbCSV);
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
                AppendHeader("Total", hasRepeta, ref sbCSV, " (nm)");

                if (WaferThicknessStat != null)
                {
                    AppendHeader("Wafer Thickness", hasRepeta, ref sbCSV, " (nm)");

                    if (WarpStat != null)
                    {
                        AppendHeader("Warp", hasRepeta, ref sbCSV, " (nm)");
                    }
                }

                foreach (var layerStat in ThicknessLayerStats)
                {
                    AppendHeader(layerStat.Key, hasRepeta, ref sbCSV, " (nm)");
                }

                sbCSV.RemoveEndDelim();
                lines.Add(sbCSV.ToString());

                int index = 1;
                foreach (var die in Dies)
                {
                    foreach (var p in die.Points.OfType<ThicknessPointResult>())
                    {
                        sbCSV.Clear();

                        sbCSV.Append($"{index}", p.State.ToHumanizedString(),
                           $"{die.ColumnIndex}", $"{die.RowIndex}",
                           $"{p.WaferRelativeXPosition}", $"{p.WaferRelativeYPosition}",
                           $"{p.XPosition}", $"{p.YPosition}");

                        AppendStatsNanometers(p.TotalThicknessStat, hasRepeta, ref sbCSV);
                        if (p.WaferThicknessStat?.Mean != null)
                        {
                            AppendStatsNanometers(p.WaferThicknessStat, hasRepeta, ref sbCSV);
                        }
                        foreach (var layerStat in ThicknessLayerStats)
                        {
                            p.ThicknessLayerStats.TryGetValue(layerStat.Key, out var container);
                            AppendStatsNanometers(container, hasRepeta, ref sbCSV);
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

        #region Calculation

        public List<Length> ComputeWarpFromRPD()
        {
            var warpRes = new List<Length>();
            bool bContinue = true;
            int i = 0;
            do
            {
                var list = Points.OfType<ThicknessPointResult>().SelectMany(x => x.RPDDatas.Skip(i).Take(1).OfType<WarpPointData>()).ToList()
                                                                .Select(y => y.RPD.GetValueAs(Settings.WarpTargetMax.Unit)).ToList();
                if (list.Count > 0)
                {
                    double rpdmax = list.Max();
                    double rpdmin = list.Min();
                    warpRes.Add(new Length(rpdmax - rpdmin, Settings.WarpTargetMax.Unit));
                }
                else
                    bContinue = false;
                ++i;
            }
            while (bContinue);
            return warpRes;
        }

        #endregion
    }
}
