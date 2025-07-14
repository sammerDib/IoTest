using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;

using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.Metro.Warp
{
    public class WarpResult : MeasureResultBase
    {
        #region Properties

        [XmlAttribute("MeasureVersion")]
        [DataMember]
        public string MeasureVersion { get; set; } = "1.0.0";

        [DataMember]
        public WarpResultSettings Settings { get; set; } = new WarpResultSettings();

		[DataMember]
        public List<Length> WarpWaferResults { get; set; } = new List<Length>();

		[DataMember]
        public List<Length> TTVWaferResults { get; set; } = new List<Length>();

        [XmlIgnore]
        public MetroStatsContainer WarpStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer TTVStat { get; private set; } // TTV = Total Thickness Variation

        [XmlIgnore]
        public MetroStatsContainer RPDStat { get; private set; } // RPD = Reference Plane Deviation

        [XmlIgnore]
        public MetroStatsContainer TotalThicknessStat { get; private set; }

        #endregion Properties

        #region Stats Generation

        public override List<ResultDataStats> GenerateStatisticsValues(long dbResId)
        {
            FillNonSerializedProperties(true, true);

            var dataStats = new List<ResultDataStats>();

            if (WarpStat != null && WarpStat.Mean != null)
            {
                // NOTE RTI pour TLA voir si on peut prendre l'unit de la target comme reférence au lieu de fixer Micrometers comme standard !

                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "Warp", WarpStat.Mean.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "Warp", WarpStat.Min.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "Warp", WarpStat.Max.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "Warp", (double)WarpStat.State, (int)UnitType.NoUnit));
            }
                
            if (TTVStat != null && TTVStat.Mean != null)
            {
                // NOTE RTI pour TLA voir si on peut prendre l'unit de la target comme reférence au lieu de fixer Micrometers comme standard !

                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "TTV", TTVStat.Mean.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "TTV", TTVStat.Min.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "TTV", TTVStat.Max.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "TTV", (double)TTVStat.State, (int)UnitType.NoUnit));
            }

            if (RPDStat != null && RPDStat.Mean != null)
            {
                // NOTE RTI pour TLA voir si on peut prendre l'unit de la target comme reférence au lieu de fixer Micrometers comme standard !

                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "RPD", RPDStat.Mean.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "RPD", RPDStat.Min.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "RPD", RPDStat.Max.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "RPD", (double)RPDStat.State, (int)UnitType.NoUnit));
            }

            if (TotalThicknessStat != null && TotalThicknessStat.Mean != null)
            {
                // NOTE RTI pour TLA voir si on peut prendre l'unit de la target comme reférence au lieu de fixer Micrometers comme standard !

                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Mean, "TT", TotalThicknessStat.Mean.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Min, "TT", TotalThicknessStat.Min.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.Max, "TT", TotalThicknessStat.Max.Micrometers, (int)UnitType.um));
                dataStats.Add(new ResultDataStats(dbResId, (int)ResultValueType.State, "TT", (double)TotalThicknessStat.State, (int)UnitType.NoUnit));
            }

            return dataStats;
        }

        public override void FillNonSerializedProperties(bool generateState, bool generateStatistics)
        {
            if (Settings == null) return;
          
            // Stats
            var RPDData = new LinkedList<MetroStatsContainer>();
            var totalThicknessData = new LinkedList<MetroStatsContainer>();
            if (generateStatistics)
            {
                QualityScore = 1.0;
            }

            void ComputeState(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is WarpPointResult point)) return;

                foreach (var data in point.WarpPointDatas)
                {
                    data.State = MeasureStateComputer.GetMeasureState_NoLimit(data.RPD);
                }
            }

            void ComputeStatistics(MeasurePointResult measurePointResult)
            {
                if (!(measurePointResult is WarpPointResult point)) return;

                point.GenerateStats();

                RPDData.AddLast(point.RPDStat);
                totalThicknessData.AddLast(point.TotalThicknessStat);

                if (point.State != MeasureState.NotMeasured && point.QualityScore < QualityScore)
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
                var warpData = new LinkedList<Tuple<Length, MeasureState>>();
                foreach (var mesure in WarpWaferResults)
                {
                    warpData.AddLast(new Tuple<Length, MeasureState>(mesure, MeasureStateComputer.GetMeasureState(mesure, new Length(0d, LengthUnit.Micrometer), Settings.WarpMax)));
                }

                var ttvData = new LinkedList<Tuple<Length, MeasureState>>();
                foreach (var mesure in TTVWaferResults)
                {
                    ttvData.AddLast(new Tuple<Length, MeasureState>(mesure, MeasureStateComputer.GetMeasureState_NoLimit(mesure)));
                }

                WarpStat = warpData.Any() ?  MetroStatsContainer.GenerateFromLength(warpData) : MetroStatsContainer.Empty;
                TTVStat = ttvData.Any() ?  MetroStatsContainer.GenerateFromLength(ttvData) : MetroStatsContainer.Empty;
                RPDStat = MetroStatsContainer.GenerateFromStats(RPDData);
                TotalThicknessStat = MetroStatsContainer.GenerateFromStats(totalThicknessData);
            }
        }

        #endregion Stats Generation

        #region CSV

        public override byte[] ToCsv()
        {
            bool isDieResult = Dies?.Count > 0;

            var lines = new List<string>();

            bool hasWarp = WarpStat != null && WarpStat.Mean != null;
            bool hasTTV = TTVStat != null && TTVStat.Mean != null;
            bool hasRPD = RPDStat != null && RPDStat.Mean != null;
            bool hasTotalThickness = TotalThicknessStat != null && TotalThicknessStat.Mean != null;

            // Regular
            if (!isDieResult)
            {
                bool hasRepeta = WarpWaferResults.Count > 1;

                var measurePointResults = Points.OfType<WarpPointResult>().ToList();
                bool hasRPDRepeta = measurePointResults.Any(result => result.Datas.Count > 1);

                // Warp + TTV Header
                var sbCSV = new CSVStringBuilder();
                AppendCSVAutomInfo(ref sbCSV);
                AppendStateAndPositionHeader(ref sbCSV);

                if (hasWarp)
                {
                    AppendHeader("Warp", hasRepeta, ref sbCSV, " (µm)");
                }

                if (hasTTV)
                {
                    AppendHeader("TTV", hasRepeta, ref sbCSV, " (µm)");
                }

                sbCSV.RemoveEndDelim();
                lines.Add(sbCSV.ToString());

                // Warp + TTV Values
                sbCSV.Clear();
                int nOffsetLine = 0;

                if (hasWarp || hasTTV)
                {
                    sbCSV.Append("1");
                    nOffsetLine++;

                    if (hasWarp)
                    {
                        sbCSV.Append($"{WarpStat.State.ToHumanizedString()}","0.0","0.0");
                        AppendStatsMicrometers(WarpStat, hasRepeta, ref sbCSV);
                    }

                    if (hasTTV)
                    {
                        AppendStatsMicrometers(TTVStat, hasRepeta, ref sbCSV);
                    }

                    AppendStats(null, hasRPDRepeta, ref sbCSV);
                    sbCSV.RemoveEndDelim();
                }

                lines.Add(sbCSV.ToString());
                sbCSV.Clear();

                // RPD + TotalThickness Values
                if (hasRPD || hasTotalThickness)
                {
                    lines.Add(sbCSV.ToString());
                    AppendStateAndPositionHeader(ref sbCSV);

                    if (hasRPD)
                    {
                        AppendHeader("RPD", hasRepeta, ref sbCSV, " (µm)");
                    }

                    if (hasTotalThickness)
                    {
                        AppendHeader("TT", hasRepeta, ref sbCSV, " (µm)");
                    }

                    sbCSV.RemoveEndDelim();
                    lines.Add(sbCSV.ToString());
                    sbCSV.Clear();

                    nOffsetLine = 0;
                    for (int i = 0; i < measurePointResults.Count; i++)
                    {
                        var p = measurePointResults[i];
                        sbCSV.Append($"{i + 1 + nOffsetLine}", p.State.ToHumanizedString(), $"{p.XPosition}", $"{p.YPosition}");

                        if (hasRPD)
                        {
                            AppendStatsMicrometers(p.RPDStat, hasRPDRepeta, ref sbCSV);
                        }

                        if (hasTotalThickness)
                        {
                            AppendStatsMicrometers(p.TotalThicknessStat, hasRPDRepeta, ref sbCSV);
                        }

                        sbCSV.RemoveEndDelim();
                        lines.Add(sbCSV.ToString()); 
                        sbCSV.Clear();
                    }
                }
            }
            // Dies
            else
            {
                throw new ApplicationException("Use Die Warpage measure instead Warp to use die");
            }

            byte[] preamble = Encoding.UTF8.GetPreamble();
            string csvContent = string.Join(Environment.NewLine, lines);
            byte[] bytes = Encoding.UTF8.GetBytes(csvContent);

            return preamble.Concat(bytes).ToArray();
        }

        #endregion CSV
    }
}
