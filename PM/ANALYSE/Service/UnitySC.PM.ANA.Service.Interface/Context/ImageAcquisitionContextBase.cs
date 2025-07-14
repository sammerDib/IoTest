using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Context
{
    [DataContract]
    public abstract class ImageAcquisitionContextBase : ANAContextBase
    {
        [DataMember]
        public LightsContext Lights { get; set; }
    }
}
