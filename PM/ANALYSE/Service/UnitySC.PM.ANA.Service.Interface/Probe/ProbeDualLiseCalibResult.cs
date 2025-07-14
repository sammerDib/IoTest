using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface
{
    [DataContract]
    public class ProbeDualLiseCalibResult : ProbeCalibResultsBase
    {

        [DataMember]
        public double ZTopUsedForCalib { get; set; }

        [DataMember]
        public double ZBottomUsedForCalib { get; set; }

        [DataMember]
        public Length AirGapUp { get; set; }

        [DataMember]
        public Length AirGapDown{ get; set; }

        [DataMember]
        public Length RefThicknessUsed { get; set; }

        [DataMember]
        public Length GlobalDistance { get; set; }

    }
}
