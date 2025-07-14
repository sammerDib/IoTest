using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.NanoTopo
{
    [DataContract]
    public class NanoTopoPointData : MeasurePointDataResultBase
    {
        [DataMember]
        public Length Roughness { get; set; }


        // TODO Must be ignored by the serialization ??
        [XmlIgnore]
        [DataMember]
        public MeasureState RoughnessState { get; set; } = MeasureState.NotMeasured;

        [DataMember]
        public Length StepHeight { get; set; }

        // TODO Must be ignored by the serialization ??
        [XmlIgnore]
        [DataMember]
        public MeasureState StepHeightState { get; set; } = MeasureState.NotMeasured;

        [DataMember]
        public List<ExternalProcessingResult> ExternalProcessingResults { get; set; }

        [DataMember]
        public string ResultImageFileName { get; set; }

        [DataMember]
        public string ReportFileName { get; set; }

        public override string ToString()
        {
            string externalProccessingResults = ExternalProcessingResults != null ? $"{nameof(ExternalProcessingResults)}: {string.Join(", ", ExternalProcessingResults)}" : string.Empty;
            return $"{base.ToString()} Roughness: {Roughness} StepHeight: {StepHeight} {externalProccessingResults}";
        }

        public override void NewIterInPath(int newIter)
        {
            ResultImageFileName = MeasurePointDataResultBaseHelper.FormatNewIterPathCopy(ResultImageFileName, newIter);
            ReportFileName = MeasurePointDataResultBaseHelper.FormatNewIterPathCopy(ReportFileName, newIter);
        }
    }
}
