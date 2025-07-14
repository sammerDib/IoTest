using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

using ProfileXY = UnitySCSharedAlgosCppWrapper.Profile2d;

namespace UnitySC.PM.ANA.Service.Interface.Profile1D
{
    [DataContract]
    public class Profile1DFlowResult : IFlowResult
    {
        [DataMember]
        public FlowStatus Status { get; set; }
        
        [DataMember]
        public ProfileXY Profile { get; set; }
    }
}
