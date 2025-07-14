using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Alignment
{
    [DataContract]
    public class BareWaferAlignmentParameters
    {
        [DataMember]
        public List<BareWaferAlignmentImagePosition> CustomImagePositions { get; set; }

        [DataMember]
        public ObjectiveContext ObjectiveContext { get; set; }
    }
}
