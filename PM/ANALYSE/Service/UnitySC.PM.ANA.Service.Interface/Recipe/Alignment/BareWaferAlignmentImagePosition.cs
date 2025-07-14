using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Alignment
{
    [DataContract]
    public class BareWaferAlignmentImagePosition
    {
        [DataMember]
        public WaferEdgePositions EdgePosition { get; set; }

        [DataMember]
        public Length X { get; set; }

        [DataMember]
        public Length Y { get; set; }
    }
}
