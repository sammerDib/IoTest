using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.TSV
{
    // Properties without [DataMember] are not serialized
    [DataContract]
    public class TSVPointResult : MeasurePointResult
    {
        [XmlIgnore]
        public MetroStatsContainer LengthTsvStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer WidthTsvStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer DepthTsvStat { get; private set; }

        [XmlIgnore]
        public List<TSVPointData> TSVDatas => Datas?.OfType<TSVPointData>().ToList();

        [XmlElement("WaferCopla")]
        [DataMember]
        public Length CoplaInWaferValue { get; set; }

        [XmlElement("DieCopla")]
        [DataMember]
        public Length CoplaInDieValue { get; set; }

        #region Private for stats generation

        public override void GenerateStats()
        {
            var lengthData = new LinkedList<Tuple<Length, MeasureState>>();
            var widthData = new LinkedList<Tuple<Length, MeasureState>>();
            var depthData = new LinkedList<Tuple<Length, MeasureState>>();
            var data = TSVDatas;

            if (data == null || data.Count == 0)
            {
                QualityScore = 0.0;
                return;
            }

            ComputeQualityScoreFromDatas();

            foreach (var tsvPointData in data)
            {
                lengthData.AddLast(new Tuple<Length, MeasureState>(tsvPointData.Length, tsvPointData.LengthState));
                widthData.AddLast(new Tuple<Length, MeasureState>(tsvPointData.Width, tsvPointData.WidthState));
                depthData.AddLast(new Tuple<Length, MeasureState>(tsvPointData.Depth, tsvPointData.DepthState));
            }

            LengthTsvStat = MetroStatsContainer.GenerateFromLength(lengthData);
            WidthTsvStat = MetroStatsContainer.GenerateFromLength(widthData);
            DepthTsvStat = MetroStatsContainer.GenerateFromLength(depthData);
        }

        #endregion Private for stats generation

        public override List<ResultValue> GetResultValues()
        {
            var tSVPointDatas = TSVDatas;
            if (tSVPointDatas == null || tSVPointDatas.Count == 0) return null;

            // on devrait afficher toujours un resultat en depit de son status, en revanche son affichage doit pouvoir gerer le cas que toutes les données ne sont pas présentes, ou mesurées
            // le code ci dessous est donc commenté, ne pas decommenté a part sur requete client
            // if (State!=MeasureState.Success) return null;

            bool useDiameter = (tSVPointDatas[0].Length == tSVPointDatas[0].Width); // workaround for Circle shape

            List<ResultValue> resultValues = null;
            if (tSVPointDatas.Count > 1)
            {
                // case of static repeta --> we should display average - +/- 3sigma - Min /Max ?
                if (DepthTsvStat == null || LengthTsvStat == null || WidthTsvStat == null)
                    GenerateStats();

                resultValues = FormatResultValues(DepthTsvStat.Mean, LengthTsvStat.Mean, WidthTsvStat.Mean, useDiameter);
            }
            else
            {
                resultValues = FormatResultValues(tSVPointDatas[0].Depth, tSVPointDatas[0].Length, tSVPointDatas[0].Width, useDiameter);
            }
            return resultValues;
        }

        private List<ResultValue> FormatResultValues(Length depth, Length length, Length width, bool useDiameter)
        {
            var resultValues = new List<ResultValue>(3);
            if (!(depth is null))
                resultValues.Add(new ResultValue() { Name = "Depth", Value = depth.Value, Unit = depth.UnitSymbol });

            if (useDiameter)
            {
                if (!(length is null))
                    resultValues.Add(new ResultValue() { Name = "Diameter", Value = length.Value, Unit = length.UnitSymbol });
            }
            else
            {
                if (!(length is null))
                    resultValues.Add(new ResultValue() { Name = "Length", Value = length.Value, Unit = length.UnitSymbol });

                if (!(width is null))
                    resultValues.Add(new ResultValue() { Name = "Width", Value = width.Value, Unit = width.UnitSymbol });
            }
            return resultValues;
        }
    
    }
}
