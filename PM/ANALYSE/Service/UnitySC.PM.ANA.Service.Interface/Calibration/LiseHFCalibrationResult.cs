
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [DataContract]
    public class LiseHFIntegrationTimeCalibrationResults : IFlowResult
    {
        public LiseHFIntegrationTimeCalibrationResults()
        {

        }

        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public List<LiseHFObjectiveIntegrationTimeCalibration> CalibIntegrationTimes { get; set; } = new List<LiseHFObjectiveIntegrationTimeCalibration>();

    }

    [DataContract]
    public class LiseHFSpotCalibrationResults : IFlowResult
    {

        public LiseHFSpotCalibrationResults()
        {

        }

        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public List<LiseHFObjectiveSpotCalibration> SpotCalibPositions { get; set; } = new List<LiseHFObjectiveSpotCalibration>();
    }

    [DataContract]
    public class LiseHFSpotCheckResult : IFlowResult
    {

        public LiseHFSpotCheckResult()
        {

        }

        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public LiseHFObjectiveSpotCalibration SpotPosition { get; set; }
    }
}
