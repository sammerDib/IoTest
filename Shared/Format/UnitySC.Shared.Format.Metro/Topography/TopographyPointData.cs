using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.Shared.Format.Metro.Topography
{
    [DataContract]
    public class TopographyPointData : MeasurePointDataResultBase
    {
        [DataMember]
        public List<ExternalProcessingResult> ExternalProcessingResults { get; set; }

        [DataMember]
        public string ResultImageFileName { get; set; }

        [DataMember]
        public string ReportFileName { get; set; }
      
        public override string ToString()
        {
            string externalProccessingResults = ExternalProcessingResults != null ? $"{nameof(ExternalProcessingResults)}: {string.Join(", ", ExternalProcessingResults)}" : string.Empty;
            return $"{base.ToString()} {externalProccessingResults}";
        }

        public override void NewIterInPath(int newIter)
        {
            ResultImageFileName =  MeasurePointDataResultBaseHelper.FormatNewIterPathCopy(ResultImageFileName, newIter);
            ReportFileName = MeasurePointDataResultBaseHelper.FormatNewIterPathCopy(ReportFileName, newIter);
        }
    }
}
