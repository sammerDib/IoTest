using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Bow
{
    // Properties without [DataMember] are not serialized
    [DataContract]
    public class BowPointResult : MeasurePointResult
    {
        [XmlIgnore]
        public List<BowPointData> BowDatas => Datas?.OfType<BowPointData>().ToList();

        /// <summary>
        /// BowTotalPointDatas are special points data computed after all sub measures
        /// And which contains global measure value
        /// </summary>
        [XmlIgnore]
        public List<BowTotalPointData> BowTotalPointDatas => Datas?.OfType<BowTotalPointData>().ToList();

        [XmlIgnore]
        public MetroStatsContainer BowStat { get; private set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            var bowData = new LinkedList<Tuple<Length, MeasureState>>();
            ComputeQualityScoreFromDatas();

            foreach (var bowPointData in BowTotalPointDatas)
            {
                bowData.AddLast(new Tuple<Length, MeasureState>(bowPointData.Bow, bowPointData.State));
            }

            BowStat = bowData.Any() ? MetroStatsContainer.GenerateFromLength(bowData) : MetroStatsContainer.Empty;
        }
        #endregion

        public override List<ResultValue> GetResultValues()
        {
            if (BowDatas != null && BowDatas.Count > 0)
            {
                return GetBowPointValues();
            }
            else if (BowTotalPointDatas != null && BowTotalPointDatas.Count > 0)
            {
                return GetBowTotalPointDataValues();
            }

            return null;
        }

        private List<ResultValue> GetBowPointValues()
        {
            var bowPointDatas = BowDatas;
            if (bowPointDatas == null || bowPointDatas.Count == 0) return null;

            // on devrait afficher toujours un resultat en depit de son status, en revanche son affichage doit pouvoir gerer le cas que toutes les données ne sont pas présentes, ou mesurées
            // le code ci dessous est donc commenté, ne pas decommenté a part sur requete client
            // if (State!=MeasureState.Success) return null;

            var resultValues = new List<ResultValue>();
            resultValues = AddResultValues(resultValues, "AirGap", bowPointDatas[0].AirGap);

            return resultValues;
        }

        private List<ResultValue> GetBowTotalPointDataValues()
        {
            var bowTotalPointDatas = BowTotalPointDatas;
            if (bowTotalPointDatas == null || bowTotalPointDatas.Count == 0) return null;

            // on devrait afficher toujours un resultat en depit de son status, en revanche son affichage doit pouvoir gerer le cas que toutes les données ne sont pas présentes, ou mesurées
            // le code ci dessous est donc commenté, ne pas decommenté a part sur requete client
            // if (State!=MeasureState.Success) return null;

            var resultValues = new List<ResultValue>();
            resultValues = AddResultValues(resultValues, "Bow", bowTotalPointDatas[0].Bow);

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
