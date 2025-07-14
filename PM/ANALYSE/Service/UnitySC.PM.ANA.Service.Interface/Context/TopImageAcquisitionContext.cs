using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    public class TopImageAcquisitionContext : ImageAcquisitionContextBase
    {
        [DataMember]
        public TopObjectiveContext TopObjectiveContext { get; set; }
    }
}
