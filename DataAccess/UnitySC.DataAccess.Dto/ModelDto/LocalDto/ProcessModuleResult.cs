using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class ProcessModuleResult
    {
        [DataMember]
        public int ChamberId { get; set; }

        [DataMember]
        public ActorType ActorType { get; set; }

        [DataMember]
        public string LabelPMName { get; set; }

        [DataMember]
        public Dictionary<string, WaferResultData[]> PostProcessingResults { get; set; }

        [DataMember]
        public Dictionary<string, WaferAcquisitionResult[]> AcquisitionResults { get; set; }

        public List<string> PostProcessingResultLabels
        {
            get
            {
                if (PostProcessingResults == null)
                    return null;

                return PostProcessingResults.Keys.ToList();
            }
        }

        public List<string> AcquisitionResultsLabels
        {
            get
            {
                if (AcquisitionResults == null)
                    return null;

                return AcquisitionResults.Keys.ToList();
            }
        }
    }
}
