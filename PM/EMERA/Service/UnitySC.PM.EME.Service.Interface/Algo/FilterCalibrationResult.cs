using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    public class FilterCalibrationResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember] public List<Filter> Filters { get; set; }
        
    }
}
