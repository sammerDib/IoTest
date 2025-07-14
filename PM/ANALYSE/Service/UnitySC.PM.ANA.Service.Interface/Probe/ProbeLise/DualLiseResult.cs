using System.Runtime.Serialization;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise
{
    [DataContract]
    public class DualLiseResult : LiseResult
    {
        #region Properties

        [DataMember]
        public Length AirGapUp { get; set; }

        [DataMember]
        public Length AirGapDown { get; set; }

        #endregion Properties
    }
}
