using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class CheckPatternRecSettings
    {
        [DataMember]
        public double ShiftRatio { get; set; }

        [DataMember]
        public int NbChecks { get; set; }
    }
}
