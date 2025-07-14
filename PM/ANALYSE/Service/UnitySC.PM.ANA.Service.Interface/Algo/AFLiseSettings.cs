using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class AFLiseSettings
    {
        [DataMember]
        public double CalibratedGainMax { get; set; }

        [DataMember]
        public double CalibratedGainMin { get; set; }

        [DataMember]
        public Length CalibratedZ { get; set; }

        [DataMember]
        public Length ZMax { get; set; }

        [DataMember]
        public Length ZMin { get; set; }
    }
}
