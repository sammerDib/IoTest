using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Topography
{
    [DataContract]
    public class TopographyPointResult : MeasurePointResult
    {
        [XmlIgnore]
        public List<TopographyPointData> TopographyDatas => Datas?.OfType<TopographyPointData>().ToList();

        [XmlIgnore]
        public Dictionary<string, MetroDoubleStatsContainer> ExternalProcessingStats { get; private set; }

        #region Overrides of MeasurePointResult

        public override void GenerateStats()
        {
            var roughnessData = new LinkedList<Tuple<Length, MeasureState>>();
            var stepHeightData = new LinkedList<Tuple<Length, MeasureState>>();
            var data = TopographyDatas;

            if (data == null || data.Count == 0)
            {
                QualityScore = 0.0;
                return;
            }

            ComputeQualityScoreFromDatas();

            var externalsProcessing = new Dictionary<string, List<Tuple<double, MeasureState>>>();

            foreach (var pointData in data)
            {
                if (pointData == null || pointData.ExternalProcessingResults == null)
                    continue;

                foreach (var processingResult in pointData.ExternalProcessingResults)
                {
                    var tuple = new Tuple<double, MeasureState>(processingResult.Value, processingResult.State);

                    if (externalsProcessing.ContainsKey(processingResult.Name))
                    {
                        externalsProcessing[processingResult.Name].Add(tuple);
                    }
                    else
                    {
                        externalsProcessing.Add(processingResult.Name, new List<Tuple<double, MeasureState>>
                        {
                            tuple
                        });
                    }
                }
            }

            ExternalProcessingStats = new Dictionary<string, MetroDoubleStatsContainer>();

            foreach (var keyValue in externalsProcessing)
            {
                string unit = string.Empty;
                if (data.Count > 0 && data[0].ExternalProcessingResults.Count > 0)
                    unit = data[0].ExternalProcessingResults[0].Unit;
                var stats = MetroDoubleStatsContainer.GenerateFromDoubles(keyValue.Value, unit);
                ExternalProcessingStats.Add(keyValue.Key, stats);
            }
        }

        public override List<ResultValue> GetResultValues()
        {
            var topographyPointsDatas = TopographyDatas;

            if (topographyPointsDatas.Count == 0)
            {
                return null;
            }

            if (State != MeasureState.Success)
            {
                return null;
            }

            int takeValues = 3;
            var resultValues = new List<ResultValue>();
            if (!(topographyPointsDatas[0].ExternalProcessingResults is null))
            {
                if (ExternalProcessingStats is null)
                {
                    GenerateStats();
                }

                foreach (var statcontainer in ExternalProcessingStats)
                {
                    var outputResultValue = new ResultValue() { Name = statcontainer.Key, Value = statcontainer.Value?.Mean ?? 0.0, Unit = String.Empty };
                    resultValues.Add(outputResultValue);
                }
            }

            if (topographyPointsDatas[0].ExternalProcessingResults != null)
            {
                foreach (var externalProcessingResult in topographyPointsDatas[0].ExternalProcessingResults.Take(takeValues))
                {
                    var externalProcessingResultValue = new ResultValue() { Name = externalProcessingResult.Name, Value = externalProcessingResult.Value, Unit = externalProcessingResult.Unit };
                    resultValues.Add(externalProcessingResultValue);
                }
            }

            return resultValues;
        }

        #endregion Overrides of MeasurePointResult
 
    }
}
