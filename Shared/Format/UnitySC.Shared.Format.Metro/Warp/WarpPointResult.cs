using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Warp
{
    // Properties without [DataMember] are not serialized
    [DataContract]
    public class WarpPointResult : MeasurePointResult
    {
        /// <summary>
        /// WarpPointDatas are the sub measure points data
        /// </summary>
        [XmlIgnore]
        internal List<WarpPointData> WarpPointDatas => Datas?.OfType<WarpPointData>().ToList();

        /// <summary>
        /// WarpTotalPointDatas are special points data computed after all sub measures
        /// And which contains global measure values (e.g. Warp & TTV)
        /// </summary>
        [XmlIgnore]
        internal List<WarpTotalPointData> WarpTotalPointDatas => Datas?.OfType<WarpTotalPointData>().ToList();

        [XmlIgnore]
        public MetroStatsContainer RPDStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer TotalThicknessStat { get; private set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            if (WarpPointDatas != null && WarpPointDatas.Count > 0)
            {
                var rpdData = new LinkedList<Tuple<Length, MeasureState>>();
                var ttData = new LinkedList<Tuple<Length, MeasureState>>();

                ComputeQualityScoreFromDatas();

                foreach (var warpPointData in WarpPointDatas)
                {
                    rpdData.AddLast(new Tuple<Length, MeasureState>(warpPointData.RPD, warpPointData.State));
                    ttData.AddLast(new Tuple<Length, MeasureState>(warpPointData.TotalThickness, warpPointData.State));
                }

                RPDStat = rpdData.Any() ? MetroStatsContainer.GenerateFromLength(rpdData) : MetroStatsContainer.Empty;
                TotalThicknessStat = ttData.Any() ? MetroStatsContainer.GenerateFromLength(ttData) : MetroStatsContainer.Empty;
            }
        }

        #endregion

        public override List<ResultValue> GetResultValues()
        {
            if (WarpPointDatas != null && WarpPointDatas.Count > 0)
            {
                return GetWarpPointValues();
            }
            else if (WarpTotalPointDatas != null && WarpTotalPointDatas.Count > 0)
            {
                return GetWarpTotalPointDataValues();
            }

            return null;
        }

        private List<ResultValue> GetWarpPointValues()
        {
            var warpPointDatas = WarpPointDatas;
            if (warpPointDatas == null || warpPointDatas.Count == 0) return null;

            // on devrait afficher toujours un resultat en depit de son status, en revanche son affichage doit pouvoir gerer le cas que toutes les données ne sont pas présentes, ou mesurées
            // le code ci dessous est donc commenté, ne pas decommenté a part sur requete client
            // if (State!=MeasureState.Success) return null;

            var resultValues = new List<ResultValue>();
            bool isSurfaceWarp = warpPointDatas[0].IsSurfaceWarp;
            if (isSurfaceWarp)
            {
                resultValues = AddResultValues(resultValues, "AirGap", warpPointDatas[0].AirGapUp);
            }
            else
            {
                resultValues = AddResultValues(resultValues, "ZMed", warpPointDatas[0].ZMedian);
                resultValues = AddResultValues(resultValues, "TT", warpPointDatas[0].TotalThickness);
            }

            return resultValues;
        }

        private List<ResultValue> GetWarpTotalPointDataValues()
        {
            var warpTotalPointDatas = WarpTotalPointDatas;
            if (warpTotalPointDatas == null || warpTotalPointDatas.Count == 0) return null;

            // on devrait afficher toujours un resultat en depit de son status, en revanche son affichage doit pouvoir gerer le cas que toutes les données ne sont pas présentes, ou mesurées
            // le code ci dessous est donc commenté, ne pas decommenté a part sur requete client
            // if (State!=MeasureState.Success) return null;

            var resultValues = new List<ResultValue>();
            resultValues = AddResultValues(resultValues, "Warp", warpTotalPointDatas[0].Warp);
            resultValues = AddResultValues(resultValues, "TTV", warpTotalPointDatas[0].TTV);

            return resultValues;
        }

        private List<ResultValue> AddResultValues(List<ResultValue> resultValues, string valueName, Length value)
        {
            if (!(value is null))
            {
                resultValues.Add(new ResultValue() { Name = valueName, Value = value.Micrometers, Unit = Length.GetUnitSymbol(LengthUnit.Micrometer) });
            }

            return resultValues;
        }
    }
}
