using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo.GetZFocus
{
    [DataContract]
    [XmlInclude(typeof(EMEContextBase))]
    public class GetZFocusInput : IEMEInputFlow
    {
        public InputValidity CheckInputValidity()
        {
            return new InputValidity(true);
        }

        public EMEContextBase InitialContext { get; set; }
        
        [DataMember]
        public double TargetDistanceSensor { get; set; }
    }
}
