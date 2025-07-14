using System;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class CalibrationLiseHFResult : IFlowResult
    {
        [DataMember]
        public ProbeLiseHFCalibResult CalibResult { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public double Quality { get; set; }
    }
}
