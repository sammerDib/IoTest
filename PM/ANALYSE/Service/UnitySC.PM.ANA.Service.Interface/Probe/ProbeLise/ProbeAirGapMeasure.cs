using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise
{
    [DataContract]
    public class ProbeAirGapMeasure
    {
        [DataMember]
        public Length AirGap { get; set; }

        [DataMember]
        public double Quality { get; set; }
    }
}
