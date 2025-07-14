using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    public class BottomImageAcquisitionContext : ImageAcquisitionContextBase
    {
        [DataMember]
        public BottomObjectiveContext BottomObjectiveContext { get; set; }
    }
}
