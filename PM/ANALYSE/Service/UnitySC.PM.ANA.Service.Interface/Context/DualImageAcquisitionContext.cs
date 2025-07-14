using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    public class DualImageAcquisitionContext : ImageAcquisitionContextBase
    {
        [DataMember]
        public BottomObjectiveContext BottomObjectiveContext { get; set; }

        [DataMember]
        public TopObjectiveContext TopObjectiveContext { get; set; }
    }
}
