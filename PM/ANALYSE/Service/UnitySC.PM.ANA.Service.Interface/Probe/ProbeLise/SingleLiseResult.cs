using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise
{
    [DataContract]
    public class SingleLiseResult : LiseResult
    {
        [DataMember]
        public Length AirGap { get; set; }
    }
}
