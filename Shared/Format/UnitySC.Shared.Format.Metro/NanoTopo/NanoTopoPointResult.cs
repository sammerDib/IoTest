using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.NanoTopo
{
    // Properties without [DataMember] are not serialized
    [DataContract]
    public class NanoTopoPointResult : MeasurePointResult
    {
        [XmlIgnore]
        public MetroStatsContainer RoughnessStat { get; private set; }

        [XmlIgnore]
        public MetroStatsContainer StepHeightStat { get; private set; }

        [XmlIgnore]
        internal List<NanoTopoPointData> NanoTopoDatas => Datas?.OfType<NanoTopoPointData>().ToList();

        [XmlIgnore]
        public Dictionary<string, MetroDoubleStatsContainer> ExternalProcessingStats { get; private set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            var roughnessData = new LinkedList<Tuple<Length, MeasureState>>();
            var stepHeightData = new LinkedList<Tuple<Length, MeasureState>>();
            var data = NanoTopoDatas;

            if (data == null || data.Count == 0)
            {
                QualityScore = 0.0;
                return;
            }

            ComputeQualityScoreFromDatas();

            var externalsProcessing = new Dictionary<string, List<Tuple<double, MeasureState>>>();

            foreach (var pointData in data)
            {
                roughnessData.AddLast(new Tuple<Length, MeasureState>(pointData.Roughness, pointData.RoughnessState));
                stepHeightData.AddLast(new Tuple<Length, MeasureState>(pointData.StepHeight, pointData.StepHeightState));

                if (!(pointData.ExternalProcessingResults is null))
                {
                    foreach (var processingResult in pointData.ExternalProcessingResults)
                    {
                        var tuple = new Tuple<double, MeasureState>(processingResult.Value, processingResult.State);

                        if (externalsProcessing.ContainsKey(processingResult.Name))
                        {
                            externalsProcessing[processingResult.Name].Add(tuple);
                        }
                        else
                        {
                            externalsProcessing.Add(processingResult.Name, new List<Tuple<double, MeasureState>> { tuple });
                        }
                    }
                }
            }

            RoughnessStat = MetroStatsContainer.GenerateFromLength(roughnessData);
            StepHeightStat = MetroStatsContainer.GenerateFromLength(stepHeightData);

            ExternalProcessingStats = new Dictionary<string, MetroDoubleStatsContainer>();

            foreach (var keyValue in externalsProcessing)
            {
                var stats = MetroDoubleStatsContainer.GenerateFromDoubles(keyValue.Value, "");
                ExternalProcessingStats.Add(keyValue.Key, stats);
            }
        }

        #endregion Overrides of MeasurePointResult

        public override List<ResultValue> GetResultValues()
        {
            if (NanoTopoDatas.Count == 0)
            {
                return null;
            }

            if (State != MeasureState.Success)
            {
                return null;
            }

            int takeValues = 3;
            var resultValues = new List<ResultValue>();
            if (!(NanoTopoDatas[0].Roughness is null))
            {
                var roughnessResultValue = new ResultValue() { Name = "Roughness", Value = NanoTopoDatas[0].Roughness.Value, Unit = NanoTopoDatas[0].Roughness.UnitSymbol };
                resultValues.Add(roughnessResultValue);
                takeValues--;
            }

            if (!(NanoTopoDatas[0].StepHeight is null))
            {
                var stepHeightResultValue = new ResultValue() { Name = "Step height", Value = NanoTopoDatas[0].StepHeight.Value, Unit = NanoTopoDatas[0].StepHeight.UnitSymbol };
                resultValues.Add(stepHeightResultValue);
                takeValues--;
            }

            if (NanoTopoDatas[0].ExternalProcessingResults != null)
            {
                foreach (var externalProcessingResult in NanoTopoDatas[0].ExternalProcessingResults.Take(takeValues))
                {
                    var externalProcessingResultValue = new ResultValue() { Name = externalProcessingResult.Name, Value = externalProcessingResult.Value, Unit = externalProcessingResult.Unit };
                    resultValues.Add(externalProcessingResultValue);
                }
            }

            return resultValues;
        }
    }
}
