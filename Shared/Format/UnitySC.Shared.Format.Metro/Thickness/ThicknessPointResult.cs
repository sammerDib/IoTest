using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Thickness
{
    // Properties without [DataMember] are not serialized
    [DataContract]
    public class ThicknessPointResult : MeasurePointResult
    {

        [XmlIgnore]
        internal List<ThicknessPointData> ThicknessDatas => Datas?.OfType<ThicknessPointData>().ToList();

        [XmlIgnore]
        public Dictionary<string, MetroStatsContainer> ThicknessLayerStats { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer TotalThicknessStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer WaferThicknessStat { get; private set; }

        [XmlIgnore]
        internal List<WarpPointData> RPDDatas  => ThicknessDatas.Select(x => x.WarpResult).Where(e => e != null).ToList();

        [XmlIgnore]
        public MetroStatsContainer RPDStat { get; private set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            var totalThicknessData = new LinkedList<Tuple<Length, MeasureState>>();
            var waferThicknessData = new LinkedList<Tuple<Length, MeasureState>>();
            
            var data = ThicknessDatas;

            if (data == null || data.Count == 0)
            {
                QualityScore = 0.0;
                return;
            }

            ComputeQualityScoreFromDatas();

            var thicknessLayerResults = new Dictionary<string, List<Tuple<Length, MeasureState>>>();
 
            foreach (var thicknesPointData in data)
            {
                totalThicknessData.AddLast(new Tuple<Length, MeasureState>(thicknesPointData.TotalThickness, thicknesPointData.TotalState));
                if (thicknesPointData.WaferThicknessResult != null)
                {
                    waferThicknessData.AddLast(new Tuple<Length, MeasureState>(thicknesPointData.WaferThicknessResult.Length, thicknesPointData.WaferThicknessResult.State));
                }

                foreach (var layerResult in thicknesPointData.ThicknessLayerResults)
                {
                    var tuple = new Tuple<Length, MeasureState>(layerResult.Length, layerResult.State);

                    if (thicknessLayerResults.ContainsKey(layerResult.Name))
                    {
                        thicknessLayerResults[layerResult.Name].Add(tuple);
                    }
                    else
                    {
                        thicknessLayerResults.Add(layerResult.Name, new List<Tuple<Length, MeasureState>>
                        {
                            tuple
                        });
                    }
                }
            }

            TotalThicknessStat = totalThicknessData.Any() ? MetroStatsContainer.GenerateFromLength(totalThicknessData) : MetroStatsContainer.Empty;
            WaferThicknessStat = waferThicknessData.Any() ? MetroStatsContainer.GenerateFromLength(waferThicknessData) : MetroStatsContainer.Empty;

            ThicknessLayerStats = new Dictionary<string, MetroStatsContainer>();
            foreach (var keyValue in thicknessLayerResults)
            {
                var stats = MetroStatsContainer.GenerateFromLength(keyValue.Value);
                ThicknessLayerStats.Add(keyValue.Key, stats);
            }

            if (RPDDatas != null && RPDDatas.Any())
            {
                var RPDData = new LinkedList<Tuple<Length, MeasureState>>();

                foreach (var RPDPointData in RPDDatas)
                {
                    RPDData.AddLast(new Tuple<Length, MeasureState>(RPDPointData.RPD, RPDPointData.State));

                    if (RPDPointData.QualityScore < QualityScore)
                    {
                        QualityScore = RPDPointData.QualityScore;
                    }
                }

                RPDStat = RPDData.Any() ? MetroStatsContainer.GenerateFromLength(RPDData) : MetroStatsContainer.Empty;
            }
        }


        public override List<ResultValue> GetResultValues()
        {
            var thicknessPointDatas = ThicknessDatas;
            if (thicknessPointDatas == null || thicknessPointDatas.Count == 0) return null;

            bool useWaferThickness = (thicknessPointDatas[0].WaferThicknessResult != null);

            List<ResultValue> resultValues = null;
            if (thicknessPointDatas.Count > 1)
            {
                // case of static repeta --> we should display average - +/- 3sigma - Min /Max 
                if (TotalThicknessStat == null)
                    GenerateStats();

                resultValues = FormatResultValuesWithRepeta(useWaferThickness);
            }
            else
            {
                resultValues = FormatResultValues(thicknessPointDatas[0], useWaferThickness);
            }
            return resultValues;
        }

        private List<ResultValue> FormatResultValuesWithRepeta(bool useWaferThickness)
        {
            int nbResults = ThicknessLayerStats.Count + (useWaferThickness ? 1 : 0);

            var resultValues = new List<ResultValue>(nbResults);

            if (ThicknessLayerStats != null)
            {
                foreach (var thicknessLayerkvp in ThicknessLayerStats)
                {
                    if (thicknessLayerkvp.Value.Mean != null)
                        resultValues.Add(new ResultValue() { Name = thicknessLayerkvp.Key, Value = thicknessLayerkvp.Value.Mean.Micrometers, Unit = Length.GetUnitSymbol(LengthUnit.Micrometer) });
                }
            }

            if (useWaferThickness)
            {
                if (!(WaferThicknessStat.Mean is null))
                    resultValues.Add(new ResultValue() { Name = "Wafer", Value = WaferThicknessStat.Mean.Micrometers, Unit = Length.GetUnitSymbol(LengthUnit.Micrometer) });
            }
            return resultValues;
        }

        private List<ResultValue> FormatResultValues(ThicknessPointData pointData, bool useWaferThickness)
        {
            int nbResults = (pointData?.ThicknessLayerResults?.Count ?? 0) + (useWaferThickness ? 1 : 0);

            var resultValues = new List<ResultValue>(nbResults);

            if (pointData?.ThicknessLayerResults != null)
            {
                foreach (var thicknessLayer in pointData.ThicknessLayerResults)
                {
                    if(thicknessLayer?.Length != null)
                       resultValues.Add(new ResultValue() { Name = thicknessLayer.Name, Value = thicknessLayer.Length.Micrometers, Unit = Length.GetUnitSymbol(LengthUnit.Micrometer) });
                }
            }

            if (useWaferThickness)
            {
                if (!(pointData is null))
                    resultValues.Add(new ResultValue() { Name = "Wafer", Value = pointData.WaferThicknessResult.Length.Micrometers, Unit = Length.GetUnitSymbol(LengthUnit.Micrometer) });
            }
            return resultValues;
        }
        #endregion
    }
}
